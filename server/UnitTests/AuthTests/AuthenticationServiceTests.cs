using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Dtos.Auth;
using Application.Services;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;
using OtpNet;

namespace UnitTests.AuthTests;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepo> _mockUserRepo = new();
    private readonly IPasswordService _passwordService = new PasswordService();
    private readonly IJwtService _jwtService;
    private readonly ICryptoService _cryptoService = new CryptoService();
    
    private readonly AuthenticationService _authenticationService;
    
    public AuthenticationServiceTests()
    {
        _jwtService = new JwtService(
            AppOptions());
        
        _authenticationService = new AuthenticationService(
            _mockUserRepo.Object,
            _passwordService,
            _jwtService,
            _cryptoService,
            TfaOptions(),
            EncryptionOptions());
    }
    
    private static IOptionsMonitor<AppOptions> AppOptions() =>
        Mock.Of<IOptionsMonitor<AppOptions>>(mock =>
            mock.CurrentValue == new AppOptions
                { JwtSecret = "e8b61c54fafb487a10a9a84b83d738bed102099bcdb775286657d2195846b774" });
    
    private static IOptionsMonitor<TfaOptions> TfaOptions()
        => Mock.Of<IOptionsMonitor<TfaOptions>>(x =>
            x.CurrentValue == new TfaOptions
            {
                Issuer = "Marketplace",
                Digits = 6,
                Period = 30
            });

    private static IOptionsMonitor<Encryption> EncryptionOptions()
        => Mock.Of<IOptionsMonitor<Encryption>>(x =>
            x.CurrentValue == new Encryption
            {
                Key = "0123456789abcdef0123456789abcdef"
            });

    /*
     * REGISTER TESTS
    */
    [Fact]
    public void Register_NewUser_ReturnsJwt()
    {
        _mockUserRepo
            .Setup(x => x.GetUserOrNull("morten@test.com"))
            .Returns((User?)null);

        _mockUserRepo
            .Setup(x => x.AddUser(It.IsAny<User>()))
            .Returns<User>(u => u);

        var result = _authenticationService.Register(new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        });

        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }
    
    [Fact]
    public void Register_ExistingUser_ThrowsValidationException()
    {
        _mockUserRepo
            .Setup(x => x.GetUserOrNull(It.IsAny<string>()))
            .Returns(new User());

        Assert.Throws<ValidationException>(() =>
            _authenticationService.Register(new RegisterRequestDto
            {
                Name = "Morten",
                Email = "morten@test.com",
                Password = "Password!123"
            }));
    }
    
    /*
     * LOGIN TESTS
    */
    [Fact]
    public void Login_ValidCredentials_ReturnsJwt()
    {
        const string password = "Password!123";
        var hash = _passwordService.HashPassword(password);

        _mockUserRepo
            .Setup(x => x.GetUserOrNull("morten@test.com"))
            .Returns(new User
            {
                Id = "1",
                Email = "morten@test.com",
                PasswordHash = hash,
                IsTfaEnabled = false
            });

        var result = _authenticationService.Login(new AuthRequestDto
        {
            Email = "morten@test.com",
            Password = password
        });

        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }

    [Fact]
    public void Login_WrongPassword_ThrowsAuthenticationException()
    {
        var hash = _passwordService.HashPassword("Password!123");

        _mockUserRepo
            .Setup(x => x.GetUserOrNull(It.IsAny<string>()))
            .Returns(new User
            {
                Email = "morten@test.com",
                PasswordHash = hash
            });

        Assert.Throws<AuthenticationException>(() =>
            _authenticationService.Login(new AuthRequestDto
            {
                Email = "morten@test.com",
                Password = "wrong"
            }));
    }
    
    [Fact]
    public void Login_TfaEnabled_ReturnsTemporaryJwt()
    {
        const string password = "Password!123";
        var hash = _passwordService.HashPassword(password);

        _mockUserRepo
            .Setup(x => x.GetUserOrNull(It.IsAny<string>()))
            .Returns(new User
            {
                Id = "1",
                Email = "morten@test.com",
                PasswordHash = hash,
                IsTfaEnabled = true
            });

        var result = _authenticationService.Login(new AuthRequestDto
        {
            Email = "morten@test.com",
            Password = password
        });

        Assert.True(result.TfaIsRequired);
        Assert.NotNull(result.Jwt);
    }
    
    /*
     * 2FA TESTS
    */
    [Fact]
    public void SetupTfa_ValidUser_ReturnsQrCode()
    {
        var user = new User
        {
            Id = "1",
            Email = "morten@test.com",
            IsTfaEnabled = false
        };

        _mockUserRepo
            .Setup(x => x.GetUserOrNull("morten@test.com"))
            .Returns(user);

        _mockUserRepo
            .Setup(x => x.UpdateUser(It.IsAny<User>()));

        var result = _authenticationService.SetupTfa(new JwtClaims
        {
            Email = "morten@test.com",
            Id = "1",
            Exp = "123",
            Type = "2FA"
            
        });

        Assert.NotNull(result.QrCodeImage);
    }
    
    [Fact]
    public void ValidateTfa_ValidCode_ReturnsJwt()
    {
        var secret = KeyGeneration.GenerateRandomKey();

        var encryptionKey = Convert.FromBase64String(
            EncryptionOptions().CurrentValue.Key);

        var encryptedSecret = _cryptoService.Encrypt(secret, encryptionKey);

        var user = new User
        {
            Id = "1",
            Email = "morten@test.com",
            IsTfaEnabled = true,
            TfaSecret = encryptedSecret.CipherText,
            Nonce = encryptedSecret.Nonce,
            Tag = encryptedSecret.Tag
        };

        _mockUserRepo
            .Setup(r => r.GetUserOrNull(user.Email))
            .Returns(user);

        var totp = new Totp(secret);
        var validCode = totp.ComputeTotp();

        var dto = new ValidateOtpRequestDto
        {
            Code = validCode
        };

        var jwt = new JwtClaims
        {
            Email = user.Email,
            Id = "1",
            Exp = "123",
            Type = "2FA"
        };
        
        var result = _authenticationService.ValidateTfa(dto, jwt);
        
        Assert.NotNull(result);
        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }
    
    [Fact]
    public void ValidateTfa_InvalidCode_ThrowsInvalidOperationException()
    {
        var secret = KeyGeneration.GenerateRandomKey();

        var encryptionKey = Convert.FromBase64String(
            EncryptionOptions().CurrentValue.Key);

        var encryptedSecret = _cryptoService.Encrypt(secret, encryptionKey);

        var user = new User
        {
            Id = "1",
            Email = "morten@test.com",
            IsTfaEnabled = true,
            TfaSecret = encryptedSecret.CipherText,
            Nonce = encryptedSecret.Nonce,
            Tag = encryptedSecret.Tag
        };

        _mockUserRepo
            .Setup(r => r.GetUserOrNull(user.Email))
            .Returns(user);

        var dto = new ValidateOtpRequestDto
        {
            Code = "000000"
        };

        var jwt = new JwtClaims
        {
            Email = user.Email,
            Id = "1",
            Exp = "123",
            Type = "2FA"
        };
        
        Assert.Throws<InvalidOperationException>(() => _authenticationService.ValidateTfa(dto, jwt));
    }
}