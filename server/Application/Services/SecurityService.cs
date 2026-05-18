using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Crypto;
using Application.Models.Dtos.Auth;
using Core.Domain.Entities;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using OtpNet;
using QRCoder;

namespace Application.Services;

public class SecurityService(IOptionsMonitor<AppOptions> appOptionsMonitor, IOptionsMonitor<Encryption> encryptionOptionsMonitor, IOptionsMonitor<TfaOptions> tfaOptions, IUserRepo repo, ICryptoService cryptoService) : ISecurityService
{
    // Argon2 settings
    private const string Name = "argon2id";
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int MemorySize = 65536; // 64MB
    private const int Iterations = 4; 
    private const int Parallelism = 2;
    
    public AuthResponseDto Login(AuthRequestDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        
        var user = repo.GetUserOrNull(normalizedEmail) ?? throw new ValidationException("Wrong email or password");
        VerifyPasswordOrThrow(dto.Password, user.PasswordHash);

        if (user.IsTfaEnabled)
        {
            return new AuthResponseDto()
            {
                TfaIsRequired = true,
                Jwt = GenerateJwt(new JwtClaims
                {
                    Id = user.Id,
                    Exp = DateTimeOffset.UtcNow.AddMinutes(5)
                        .ToUnixTimeSeconds()
                        .ToString(),
                    Email = user.Email,
                    Type = "2FA"
                })
            };
        }
      
        return new AuthResponseDto
        {
            Jwt = GenerateJwt(new JwtClaims
            {
                Id = user.Id,
                Exp = DateTimeOffset.UtcNow.AddHours(1)
                    .ToUnixTimeSeconds()
                    .ToString(),
                Email = dto.Email,
                Type = "Auth"
            })
        };
    }

    public TfaSetupResponseDto SetupTfa(JwtClaims jwt)
    {
        var key = KeyGeneration.GenerateRandomKey();
        var base32Key = Base32Encoding.ToString(key);

        var escapedIssuer = Uri.EscapeDataString(tfaOptions.CurrentValue.Issuer);
        var escapedUser = Uri.EscapeDataString(jwt.Email);
        var otpUri = $"otpauth://totp/{escapedIssuer}:{escapedUser}?secret={base32Key}&issuer={escapedIssuer}&digits={tfaOptions.CurrentValue.Digits}&period={tfaOptions.CurrentValue.Period}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(10);

        var user = repo.GetUserOrNull(jwt.Email) ?? throw new InvalidOperationException("User not found");
        
        if (user.IsTfaEnabled) throw new InvalidOperationException("2FA is already enabled for this user");
        
        user.UpdatedAt = DateTime.UtcNow;
        user.IsTfaEnabled = true;
        var encryptionKey = Convert.FromBase64String(encryptionOptionsMonitor.CurrentValue.Key);
        var encryptedTfa = cryptoService.Encrypt(key, encryptionKey);
        user.TfaSecret = encryptedTfa.CipherText;
        user.Tag = encryptedTfa.Tag;
        user.Nonce = encryptedTfa.Nonce;
        repo.UpdateUser(user);
        return new TfaSetupResponseDto
        {
            QrCodeImage = qrCodeImage
        };
    }

    public AuthResponseDto ValidateTfa(ValidateOtpRequestDto dto, JwtClaims jwt)
    {
        var user = repo.GetUserOrNull(jwt.Email) ?? throw new InvalidOperationException("User not found");
        if (user.TfaSecret is null || !user.IsTfaEnabled || user.Nonce is null || user.Tag is null) throw new InvalidOperationException("2FA not set up for this user");
        
        var totp = new Totp(cryptoService.Decrypt(new EncryptedMessage(user.TfaSecret, user.Nonce, user.Tag), Convert.FromBase64String(encryptionOptionsMonitor.CurrentValue.Key)));
        var isValid = totp.VerifyTotp(
            dto.Code, 
            out var timeStepMatched, 
            VerificationWindow.RfcSpecifiedNetworkDelay);

        if (!isValid)
            throw new InvalidOperationException("Invalid code");
        
        return new AuthResponseDto()
        {
            Jwt = GenerateJwt(new JwtClaims
            {
                Id = user.Id,
                Exp = DateTimeOffset.UtcNow.AddHours(1)
                    .ToUnixTimeSeconds()
                    .ToString(),
                Email = user.Email,
                Type = "Auth"
            })
        };
    }

    public AuthResponseDto Register(RegisterRequestDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        
        var existingUser = repo.GetUserOrNull(normalizedEmail);
        if (existingUser is not null) throw new ValidationException("User already exists");
        
        var hash = HashPassword(dto.Password);
        
        var insertedUser = repo.AddUser(new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Email = normalizedEmail,
            PasswordHash = hash
        });
        return new AuthResponseDto
        {
            Jwt = GenerateJwt(new JwtClaims
            {
                Id = insertedUser.Id,
                Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString(),
                Email = insertedUser.Email,
                Type = "Auth"
            })
        };
    }
    
    private byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

    public string HashPassword(string password)
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

    public void VerifyPasswordOrThrow(string password, string storedHash)
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

    private static byte[] Decode(string value)
    {
        return Convert.FromBase64String(value);
    }

    private static string Encode(byte[] value)
    {
        return Convert.ToBase64String(value);
    }

    public string GenerateJwt(JwtClaims claims)
    {
        var tokenBuilder = new JwtBuilder()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .WithSecret(appOptionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer());

        foreach (var claim in claims.GetType().GetProperties())
            tokenBuilder.AddClaim(claim.Name, claim.GetValue(claims)!.ToString());
        return tokenBuilder.Encode();
    }

    public JwtClaims VerifyJwtOrThrow(string jwt)
    {
        var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .WithSecret(appOptionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
            .MustVerifySignature()
            .Decode<JwtClaims>(jwt);

        if (DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)) < DateTimeOffset.UtcNow)
            throw new AuthenticationException("Token expired");
        return token;
    }
}