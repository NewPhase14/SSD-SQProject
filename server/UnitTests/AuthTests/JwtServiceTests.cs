using System.Security.Authentication;
using Application.Models;
using Application.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests.AuthTests;

public class JwtServiceTests
{
    private readonly JwtService _jwtService = new(AppOptions());

    private static IOptionsMonitor<AppOptions> AppOptions() =>
        Mock.Of<IOptionsMonitor<AppOptions>>(x =>
            x.CurrentValue == new AppOptions
            {
                JwtSecret = "e8b61c54fafb487a10a9a84b83d738bed102099bcdb775286657d2195846b774"
            });
    
    [Fact]
    public void GenerateJwt_ReturnsToken()
    {
        // Act
        var token = _jwtService.GenerateJwt(new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        });

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateJwt_AndVerify_ReturnsSameClaims()
    {
        // Arrange
        var claims = new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        };

        // Act
        var token = _jwtService.GenerateJwt(claims);
        var result = _jwtService.VerifyJwtOrThrow(token);

        // Assert
        Assert.Equal(claims.Id, result.Id);
        Assert.Equal(claims.Email, result.Email);
        Assert.Equal(claims.Type, result.Type);
        Assert.Equal(claims.Exp, result.Exp);
    }

    [Fact]
    public void VerifyJwt_TamperedToken_Throws()
    {
        // Arrange
        var token = _jwtService.GenerateJwt(new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        });

        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
            _jwtService.VerifyJwtOrThrow(token + "abc"));
    }

    [Fact]
    public void VerifyJwt_ExpiredToken_Throws()
    {
        // Arrange — set expiry in the past to simulate an expired token
        var token = _jwtService.GenerateJwt(new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds().ToString()
        });

        // Act & Assert
        Assert.Throws<AuthenticationException>(() =>
            _jwtService.VerifyJwtOrThrow(token));
    }
}