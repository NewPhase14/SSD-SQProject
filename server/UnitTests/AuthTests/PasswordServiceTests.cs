using System.Security.Authentication;
using Application.Services;

namespace UnitTests.AuthTests;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService = new();

    [Fact]
    public void HashPassword_ValidPassword_ReturnsThreePartString()
    {
        var hash = _passwordService.HashPassword("Password!123");

        var parts = hash.Split('$');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void HasPassword_SamePassword_ReturnsDifferentHashes()
    {
        var hash1 = _passwordService.HashPassword("Password!123");
        var hash2 = _passwordService.HashPassword("Password!123");

        Assert.NotEqual(hash1, hash2);
    }
    
    [Fact]
    public void VerifyPasswordOrThrow_CorrectPassword_DoesNotThrow()
    {
        const string password = "Password!123";
        var hash = _passwordService.HashPassword(password);

        var exception = Record.Exception(() => _passwordService.VerifyPasswordOrThrow(password, hash));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyPasswordOrThrow_WrongPassword_ThrowsAuthenticationException()
    {
        const string password = "Password!123";
        const string wrongPassword = "WrongPassword!123";
        
        var hash = _passwordService.HashPassword(password);
        
        Assert.Throws<AuthenticationException>(() =>
            _passwordService.VerifyPasswordOrThrow(wrongPassword, hash));
    }
}