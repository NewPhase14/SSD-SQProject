using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Application.Models.Crypto;

namespace Application.Services;

public class CryptoService : ICryptoService
{
    
    public EncryptedMessage Encrypt(string plainText, byte[] key)
    {
        return Encrypt(Encoding.UTF8.GetBytes(plainText), key);
    }

    public EncryptedMessage Encrypt(byte[] plainText, byte[] key)
    {
        using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // MaxSize = 12
        RandomNumberGenerator.Fill(nonce);
        var ciphertext = new byte[plainText.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // MaxSize = 16
        aes.Encrypt(nonce, plainText, ciphertext, tag);
        return new EncryptedMessage(ciphertext, nonce, tag);
    }

    public string DecryptString(EncryptedMessage msg, byte[] key)
    {
        return Encoding.UTF8.GetString(Decrypt(msg, key));
    }

    public byte[] Decrypt(EncryptedMessage msg, byte[] key)
    {
        using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
        var plaintextBytes = new byte[msg.CipherText.Length];
        aes.Decrypt(msg.Nonce, msg.CipherText, msg.Tag, plaintextBytes);
        return plaintextBytes;
    }
    
}