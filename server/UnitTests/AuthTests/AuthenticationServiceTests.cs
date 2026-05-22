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
    public async Task Register_NewUser_ReturnsJwt()
    {
        // Arrange
        _mockUserRepo
            .Setup(x => x.GetUserByEmailAsync("morten@test.com"))
            .ReturnsAsync((User?)null);

        _mockUserRepo
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _authenticationService.Register(new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        });

        // Assert
        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }
    
    [Fact]
    public async Task Register_ExistingUser_ThrowsValidationException()
    {
        // Arrange — repo returns an existing user, simulating a duplicate email
        _mockUserRepo
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await _authenticationService.Register(new RegisterRequestDto
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
    public async Task Login_ValidCredentials_ReturnsJwt()
    {
        // Arrange
        const string password = "Password!123";
        var hash = _passwordService.HashPassword(password);

        _mockUserRepo
            .Setup(x => x.GetUserByEmailAsync("morten@test.com"))
            .ReturnsAsync(new User
            {
                Id = "1",
                Email = "morten@test.com",
                PasswordHash = hash,
                IsTfaEnabled = false
            });

        // Act
        var result = await _authenticationService.Login(new AuthRequestDto
        {
            Email = "morten@test.com",
            Password = password
        });

        // Assert
        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsAuthenticationException()
    {
        // Arrange
        var hash = _passwordService.HashPassword("Password!123");

        _mockUserRepo
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User
            {
                Email = "morten@test.com",
                PasswordHash = hash
            });

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(async () =>
            await _authenticationService.Login(new AuthRequestDto
            {
                Email = "morten@test.com",
                Password = "wrong"
            }));
    }
    
    [Fact]
    public async Task Login_TfaEnabled_ReturnsTemporaryJwt()
    {
        // Arrange — user has 2FA enabled, so login should return a short-lived 2FA token
        // instead of a full Auth token. The client must complete the TOTP step to get an Auth token.
        const string password = "Password!123";
        var hash = _passwordService.HashPassword(password);

        _mockUserRepo
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User
            {
                Id = "1",
                Email = "morten@test.com",
                PasswordHash = hash,
                IsTfaEnabled = true
            });

        // Act
        var result = await _authenticationService.Login(new AuthRequestDto
        {
            Email = "morten@test.com",
            Password = password
        });

        // Assert — TfaIsRequired signals the client to prompt for a TOTP code
        Assert.True(result.TfaIsRequired);
        Assert.NotNull(result.Jwt);
    }
    
    /*
     * 2FA TESTS
    */
    
    [Fact]
    public async Task SetupTfa_ValidUser_ReturnsQrCode()
    {
        // Arrange
        var user = new User
        {
            Id = "1",
            Email = "morten@test.com",
            IsTfaEnabled = false
        };

        _mockUserRepo
            .Setup(x => x.GetUserByEmailAsync("morten@test.com"))
            .ReturnsAsync(user);

        _mockUserRepo
            .Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _authenticationService.SetupTfa(new JwtClaims
        {
            Email = "morten@test.com",
            Id = "1",
            Exp = "123",
            Type = "2FA"
        });

        // Assert — QR code is returned as a PNG byte array for the client to display
        Assert.NotNull(result.QrCodeImage);
    }
    
    [Fact]
    public async Task ValidateTfa_ValidCode_ReturnsJwt()
    {
        // Arrange — encrypt the TOTP secret the same way the service does,
        // so the service can decrypt and verify the TOTP code correctly
        var secret = KeyGeneration.GenerateRandomKey();
        var encryptionKey = Convert.FromBase64String(EncryptionOptions().CurrentValue.Key);
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
            .Setup(r => r.GetUserByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var totp = new Totp(secret);
        var validCode = totp.ComputeTotp();

        // Act
        var result = await _authenticationService.ValidateTfa(
            new ValidateOtpRequestDto { Code = validCode },
            new JwtClaims { Email = user.Email, Id = "1", Exp = "123", Type = "2FA" });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }
    
    [Fact]
    public async Task ValidateTfa_InvalidCode_ThrowsInvalidOperationException()
    {
        // Arrange — "000000" is an intentionally wrong TOTP code
        var secret = KeyGeneration.GenerateRandomKey();
        var encryptionKey = Convert.FromBase64String(EncryptionOptions().CurrentValue.Key);
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
            .Setup(r => r.GetUserByEmailAsync(user.Email))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _authenticationService.ValidateTfa(
                new ValidateOtpRequestDto { Code = "000000" },
                new JwtClaims { Email = user.Email, Id = "1", Exp = "123", Type = "2FA" }));
    }
}