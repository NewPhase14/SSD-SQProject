using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Konscious.Security.Cryptography;

namespace Application.Services;

public class PasswordService : IPasswordService
{
    // Argon2 settings
    private const string Name = "argon2id";
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int MemorySize = 65536; // 64MB
    private const int Iterations = 4; 
    private const int Parallelism = 2;

    public override string HashPassword(string password)
    {
        var salt = GenerateSalt();
        
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        
        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            // CPU cost
            Iterations = Iterations,
            // RAM cost
            MemorySize = MemorySize,
            // Parallel CPU threads
            DegreeOfParallelism = Parallelism
        };

        // Generate final hash bytes
        var hash = argon2.GetBytes(HashSize);
        
        return $"{Name}${Encode(salt)}${Encode(hash)}";
    }

    public override void VerifyPasswordOrThrow(string password, string storedHash)
    {
        var parts = storedHash.Split('$');
        if (parts.Length != 3)
            throw new ValidationException("Wrong email or password");
        
        var saltEncoded = parts[1];
        var hashEncoded = parts[2];
        
        var salt = Decode(saltEncoded);
        var expectedHash = Decode(hashEncoded);
        
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        
        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            Iterations = Iterations,
            MemorySize = MemorySize,
            DegreeOfParallelism = Parallelism
        };
        
        var actualHash = argon2.GetBytes(HashSize);
        
        var verified = CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash
        );
        
        if (!verified)
            throw new AuthenticationException("Wrong email or password");
    }
    
    private byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }
    
    private static byte[] Decode(string value)
    {
        return Convert.FromBase64String(value);
    }

    private static string Encode(byte[] value)
    {
        return Convert.ToBase64String(value);
    }
}