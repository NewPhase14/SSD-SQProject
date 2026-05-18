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
using Microsoft.Extensions.Options;
using OtpNet;
using QRCoder;

namespace Application.Services;

public class SecurityService(IOptionsMonitor<AppOptions> appOptionsMonitor, IOptionsMonitor<Encryption> encryptionOptionsMonitor, IOptionsMonitor<TfaOptions> tfaOptions, IUserRepo repo, ICryptoService cryptoService) : ISecurityService
{
    public AuthResponseDto Login(AuthRequestDto dto)
    {
        var user = repo.GetUserOrNull(dto.Email) ?? throw new ValidationException("Wrong email or password");
        VerifyPasswordOrThrow(dto.Password + user.PasswordSalt, user.PasswordHash);

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
        var user = repo.GetUserOrNull(dto.Email);
        if (user is not null) throw new ValidationException("User already exists");
        var salt = GenerateSalt();
        var hash = HashPassword(dto.Password + salt);
        var insertedUser = repo.AddUser(new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordSalt = salt,
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

    /// <summary>
    ///     Gives hex representation of SHA512 hash
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public string HashPassword(string password)
    {
        using var sha512 = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha512.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public void VerifyPasswordOrThrow(string password, string hashedPassword)
    {
        if (HashPassword(password) != hashedPassword)
            throw new AuthenticationException("Invalid login");
    }

    public string GenerateSalt()
    {
        return Guid.NewGuid().ToString();
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