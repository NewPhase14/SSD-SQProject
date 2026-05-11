namespace Application.Models.Crypto;

public record EncryptedMessage(
    byte[] CipherText,
    byte[] Nonce,
    byte[] Tag
);