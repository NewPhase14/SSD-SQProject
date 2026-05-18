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
        var key = GenerateKey();
        var plainText = "Same message";

        var encrypted1 = _cryptoService.Encrypt(plainText, key);
        var encrypted2 = _cryptoService.Encrypt(plainText, key);

        Assert.NotEqual(encrypted1.CipherText, encrypted2.CipherText);
    }
    
    [Fact]
    public void Encrypt_SamePlainText_ProducesDifferentNonce()
    {
        var key = GenerateKey();
        var plainText = "Same message";

        var encrypted1 = _cryptoService.Encrypt(plainText, key);
        var encrypted2 = _cryptoService.Encrypt(plainText, key);

        Assert.NotEqual(encrypted1.Nonce, encrypted2.Nonce);
    }
    
    [Fact]
    public void Decrypt_WrongKey_ThrowsAuthenticationTagMismatchException()
    {
        var key = GenerateKey();
        var wrongKey = GenerateKey();

        var encrypted = _cryptoService.Encrypt("secret", key);

        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, wrongKey));
    }   

    [Fact]
    public void Decrypt_TamperedCiphertext_ThrowsAuthenticationTagMismatchException()
    {
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);
    
        encrypted.CipherText[0] ^= 0xFF;
    
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, key));
    }
    
    [Fact]
    public void Decrypt_TamperedTag_ThrowsAuthenticationTagMismatchException()
    {
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);
    
        encrypted.Tag[0] ^= 0xFF;
    
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, key));
    }
    
    [Fact]
    public void Decrypt_TamperedNonce_ThrowsAuthenticationTagMismatchException()
    {
        var key = GenerateKey();
        var encrypted = _cryptoService.Encrypt("secret", key);
    
        encrypted.Nonce[0] ^= 0xFF;
    
        Assert.Throws<AuthenticationTagMismatchException>(() =>
            _cryptoService.Decrypt(encrypted, key));
    }
    
    [Fact]
    public void DecryptString_ReturnsCorrectString()
    {
        var key = GenerateKey();
        var plainText = "Hello, World!";
    
        var encrypted = _cryptoService.Encrypt(plainText, key);
        var decrypted = _cryptoService.DecryptString(encrypted, key);
    
        Assert.Equal(plainText, decrypted);
    }
}
        
        
        
    
    
    