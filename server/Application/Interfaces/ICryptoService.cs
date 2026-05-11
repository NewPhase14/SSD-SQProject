using Application.Models.Crypto;
using Application.Services;
namespace Application.Interfaces;

public interface ICryptoService
{
    public EncryptedMessage Encrypt(string plainText, byte[] key);

    public EncryptedMessage Encrypt(byte[] plainText, byte[] key);

    public string DecryptString(EncryptedMessage msg, byte[] key);

    public byte[] Decrypt(EncryptedMessage msg, byte[] key);
}