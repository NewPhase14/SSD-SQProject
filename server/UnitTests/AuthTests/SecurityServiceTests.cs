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

namespace UnitTests.AuthTests;

public class SecurityServiceTests
{
    private readonly ISecurityService _securityService;
    private readonly Mock<IUserRepo> _mockUserRepo;

    
    private static IOptionsMonitor<AppOptions> OptionsMonitor() =>
        Mock.Of<IOptionsMonitor<AppOptions>>(mock =>
            mock.CurrentValue == new AppOptions
                { JwtSecret = "e8b61c54fafb487a10a9a84b83d738bed102099bcdb775286657d2195846b774" });

    public SecurityServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepo>();

        _securityService = new SecurityService(OptionsMonitor(), _mockUserRepo.Object);
    }
    
    [Fact]
    public void HashPassword_ValidPassword_ReturnsThreePartString()
    {
        var hash = _securityService.HashPassword("Password!123");

        var parts = hash.Split('$');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void HasPassword_SamePassword_ReturnsDifferentHashes()
    {
        var hash1 = _securityService.HashPassword("Password!123");
        var hash2 = _securityService.HashPassword("Password!123");

        Assert.NotEqual(hash1, hash2);
    }
    
    [Fact]
    public void VerifyPasswordOrThrow_CorrectPassword_DoesNotThrow()
    {
        var password = "Password!123";
        var hash = _securityService.HashPassword(password);

        var exception = Record.Exception(() => _securityService.VerifyPasswordOrThrow(password, hash));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyPasswordOrThrow_WrongPassword_ThrowsAuthenticationException()
    {
        var password = "Password!123";
        var wrongPassword = "WrongPassword!123";
        
        var hash = _securityService.HashPassword(password);
        var exception = Record.Exception(() => _securityService.VerifyPasswordOrThrow(wrongPassword, hash));
        Assert.NotNull(exception);
    }
    
    [Fact]
    public void GenerateJwt_AndVerify_RoundTrip_ReturnsCorrectClaims()
    {
        var claims = new JwtClaims
        {
            Id = "123",
            Email = "morten@test.com",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        };

        var jwt = _securityService.GenerateJwt(claims);
        var decoded = _securityService.VerifyJwtOrThrow(jwt);

        Assert.Equal(claims.Id, decoded.Id);
        Assert.Equal(claims.Email, decoded.Email);
        Assert.Equal(claims.Exp, decoded.Exp);
    }

    [Fact]
    public void VerifyJwtOrThrow_ExpiredToken_ThrowsAuthenticationException()
    {
        var claims = new JwtClaims
        {
            Id = "user-123",
            Email = "morten@test.com",
            Exp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds().ToString() // expired
        };

        var jwt = _securityService.GenerateJwt(claims);

        Assert.Throws<AuthenticationException>(() => _securityService.VerifyJwtOrThrow(jwt));
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsJwt()
    {
        var password = "Password!123";
        var hash = _securityService.HashPassword(password);

        _mockUserRepo
            .Setup(r => r.GetUserOrNull("morten@test.com"))
            .Returns(new User { Id = "1", Email = "morten@test.com", PasswordHash = hash });

        var result = _securityService.Login(new AuthRequestDto
        {
            Email = "morten@test.com",
            Password = password
        });

        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }
    
    [Fact]
    public void Login_UnknownEmail_ThrowsValidationException()
    {
        _mockUserRepo
            .Setup(r => r.GetUserOrNull(It.IsAny<string>()))
            .Returns((User?)null);

        Assert.Throws<ValidationException>(() => _securityService.Login(new AuthRequestDto
        {
            Email = "torben@test.com",
            Password = "Password!123"
        }));
    }

    [Fact]
    public void Login_WrongPassword_ThrowsAuthenticationException()
    {
        var hash = _securityService.HashPassword("Password!123");

        _mockUserRepo
            .Setup(r => r.GetUserOrNull("morten@test.com"))
            .Returns(new User { Id = "user-1", Email = "morten@test.com", PasswordHash = hash });

        Assert.Throws<AuthenticationException>(() => _securityService.Login(new AuthRequestDto
        {
            Email = "morten@test.com",
            Password = "WrongPassword!999"
        }));
    }

    [Fact]
    public void Register_NewUser_ReturnsJwt()
    {
        _mockUserRepo
            .Setup(r => r.GetUserOrNull("morten@test.com"))
            .Returns((User?)null);

        _mockUserRepo
            .Setup(r => r.AddUser(It.IsAny<User>()))
            .Returns<User>(u => u);

        var result = _securityService.Register(new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        });

        Assert.NotNull(result.Jwt);
        Assert.NotEmpty(result.Jwt);
    }

    [Fact]
    public void Register_ExistingEmail_ThrowsValidationException()
    {
        _mockUserRepo
            .Setup(r => r.GetUserOrNull("morten@test.com"))
            .Returns(new User { Id = "user-1", Email = "morten@test.com", PasswordHash = "x" });

        Assert.Throws<ValidationException>(() => _securityService.Register(new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        }));
    }
    
    [Fact]
    public void Register_StoredPasswordIsHashed_NotPlaintext()
    {
        _mockUserRepo
            .Setup(r => r.GetUserOrNull(It.IsAny<string>()))
            .Returns((User?)null);

        User? captured = null;
        _mockUserRepo
            .Setup(r => r.AddUser(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .Returns<User>(u => u);

        _securityService.Register(new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        });

        Assert.NotEqual("Password!123", captured!.PasswordHash);
    }









}