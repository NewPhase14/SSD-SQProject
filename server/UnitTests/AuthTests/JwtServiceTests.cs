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
        var token = _jwtService.GenerateJwt(new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        });

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateJwt_AndVerify_ReturnsSameClaims()
    {
        var claims = new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        };

        var token = _jwtService.GenerateJwt(claims);
        var result = _jwtService.VerifyJwtOrThrow(token);

        Assert.Equal(claims.Id, result.Id);
        Assert.Equal(claims.Email, result.Email);
        Assert.Equal(claims.Type, result.Type);
        Assert.Equal(claims.Exp, result.Exp);
    }

    [Fact]
    public void VerifyJwt_TamperedToken_Throws()
    {
        var token = _jwtService.GenerateJwt(new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        });

        var tampered = token + "abc";

        Assert.ThrowsAny<Exception>(() =>
            _jwtService.VerifyJwtOrThrow(tampered));
    }

    [Fact]
    public void VerifyJwt_ExpiredToken_Throws()
    {
        var token = _jwtService.GenerateJwt(new JwtClaims
        {
            Id = "1",
            Email = "test@test.com",
            Type = "Auth",
            Exp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds().ToString()
        });

        Assert.Throws<AuthenticationException>(() =>
            _jwtService.VerifyJwtOrThrow(token));
    }
}