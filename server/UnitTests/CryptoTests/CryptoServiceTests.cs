using System.Security.Cryptography;
using Application.Interfaces;
using Application.Services;

namespace UnitTests.CryptoTests;

public class CryptoServiceTests
{
    private readonly ICryptoService _cryptoService =  new CryptoService();
    
    // AES-GCM requires a 256-bit (32 byte) key
    private static byte[] GenerateKey() => RandomNumberGenerator.GetBytes(32);
    
    [Fact]
    public void Encrypt_SamePlainText_ProducesDifferentCiphertexts()
    {
        // Arange
        var key = GenerateKey();
        var plainText = "Same message";

        // Act
        var encrypted1 = _cryptoService.Encrypt(plainText, key);
        var encrypted2 = _cryptoService.Encrypt(plainText, key);

        // Assert
        Assert.NotEqual(encrypted1.CipherText, encrypted2.CipherText);
    }
    
    [Fact]
    public void Encrypt_SamePlainText_ProducesDifferentNonce()
    {
        // Arange
        var key = GenerateKey();
        var plainText = "Same message";

        // Act
        var encrypted1 = _cryptoService.Encrypt(plainText, key);
        var encrypted2 = _cryptoService.Encrypt(plainText, key);
        
        // Assert
        Assert.NotEqual(encrypted1.Nonce, encrypted2.Nonce);
    }
    
    [Fact]
    public void Decrypt_WrongKey_ThrowsAuthenticationTagMismatchException()
    {
        // Arrange
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);

        // Act & Assert — GCM authentication tag verification fails with a wrong key
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, GenerateKey()));
    }

    [Fact]
    public void Decrypt_TamperedCiphertext_ThrowsAuthenticationTagMismatchException()
    {
        // Arrange
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);
        encrypted.CipherText[0] ^= 0xFF;
    
        // Act & Assert - GCM detects integrity violation and throws AuthenticationTagMismatchException
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, key));
    }
    
    [Fact]
    public void Decrypt_TamperedTag_ThrowsAuthenticationTagMismatchException()
    {
        // Arrange
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);
        encrypted.Tag[0] ^= 0xFF;
    
        // Act & Assert - GCM detects integrity violation and throws AuthenticationTagMismatchException
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, key));
    }
    
    [Fact]
    public void Decrypt_TamperedNonce_ThrowsAuthenticationTagMismatchException()
    {
        // Arrange
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);
        encrypted.Nonce[0] ^= 0xFF;
        
        // Act & Assert - GCM detects integrity violation and throws AuthenticationTagMismatchException
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, key));
    }
    
    [Fact]
    public void DecryptString_ReturnsCorrectString()
    {
        // Arrange
        var key = GenerateKey();
        var plainText = "Hello, World!";
    
        // Act
        var encrypted = _cryptoService.Encrypt(plainText, key);
        var decrypted = _cryptoService.DecryptString(encrypted, key);
        
        //Assert
        Assert.Equal(plainText, decrypted);
    }
}
        
        
        
    
    
    