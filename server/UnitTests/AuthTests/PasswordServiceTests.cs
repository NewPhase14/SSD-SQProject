using System.Security.Authentication;
using Application.Services;

namespace UnitTests.AuthTests;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService = new();

    [Fact]
    public void HashPassword_ValidPassword_ReturnsThreePartString()
    {
        // Act
        var hash = _passwordService.HashPassword("Password!123");
        var parts = hash.Split('$');
        
        // Assert
        Assert.Equal(3, parts.Length);
    }
    
    [Fact]
    public void HashPassword_SamePassword_ReturnsDifferentHashes()
    {
        // Act — hash the same password twice; Argon2id uses a random salt each time
        var hash1 = _passwordService.HashPassword("Password!123");
        var hash2 = _passwordService.HashPassword("Password!123");

        // Assert — different salts must produce different hashes
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPasswordOrThrow_CorrectPassword_DoesNotThrow()
    {
        // Arrange
        var hash = _passwordService.HashPassword("Password!123");

        // Act & Assert
        var exception = Record.Exception(() =>
            _passwordService.VerifyPasswordOrThrow("Password!123", hash));

        Assert.Null(exception);
    }

    [Fact]
    public void VerifyPasswordOrThrow_WrongPassword_ThrowsAuthenticationException()
    {
        // Arrange
        var hash = _passwordService.HashPassword("Password!123");

        // Act & Assert
        Assert.Throws<AuthenticationException>(() =>
            _passwordService.VerifyPasswordOrThrow("WrongPassword!123", hash));
    }
}