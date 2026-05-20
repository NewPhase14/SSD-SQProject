using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Crypto;
using Application.Models.Dtos.Auth;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using OtpNet;
using QRCoder;

namespace Application.Services;

public class AuthenticationService(IUserRepo repo,
    IPasswordService passwordService,
    IJwtService jwtService, ICryptoService cryptoService,
    IOptionsMonitor<TfaOptions> tfaOptions,
    IOptionsMonitor<Encryption> encryptionOptions ) : IAuthenticationService
{
    public AuthResponseDto Register(RegisterRequestDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        
        var existingUser = repo.GetUserOrNull(normalizedEmail);
        if (existingUser is not null) throw new ValidationException("User already exists");
        
        var hash = passwordService.HashPassword(dto.Password);
        
        var insertedUser = repo.AddUser(new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Email = normalizedEmail,
            PasswordHash = hash
        });
        return new AuthResponseDto
        {
            Jwt = jwtService.GenerateJwt(new JwtClaims
            {
                Id = insertedUser.Id,
                Exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString(),
                Email = insertedUser.Email,
                Type = "Auth"
            })
        };
    }

    public AuthResponseDto Login(AuthRequestDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        
        var user = repo.GetUserOrNull(normalizedEmail) ?? throw new ValidationException("Wrong email or password");
        passwordService.VerifyPasswordOrThrow(dto.Password, user.PasswordHash);

        if (user.IsTfaEnabled)
        {
            return new AuthResponseDto()
            {
                TfaIsRequired = true,
                Jwt = jwtService.GenerateJwt(new JwtClaims                
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
            Jwt = jwtService.GenerateJwt(new JwtClaims
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

        var qrCodeImage = GenerateQrCode(
            jwt.Email,
            key);

        var user = repo.GetUserOrNull(jwt.Email)
                   ?? throw new InvalidOperationException(
                       "User not found");

        if (user.IsTfaEnabled)
            throw new InvalidOperationException(
                "2FA is already enabled for this user");

        user.UpdatedAt = DateTime.UtcNow;
        user.IsTfaEnabled = true;
        
        var encryptedTfa = cryptoService.Encrypt(
            key,
            GetEncryptionKey());

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
        
        var encryptedMessage = new EncryptedMessage(
            user.TfaSecret,
            user.Nonce,
            user.Tag);

        var decryptedSecret = cryptoService.Decrypt(
            encryptedMessage,
            GetEncryptionKey());

        var totp = new Totp(decryptedSecret);
        var isValid = totp.VerifyTotp(
            dto.Code, 
            out var timeStepMatched, 
            VerificationWindow.RfcSpecifiedNetworkDelay);

        if (!isValid)
            throw new InvalidOperationException("Invalid code");
        
        return new AuthResponseDto()
        {
            Jwt = jwtService.GenerateJwt(new JwtClaims
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

    private byte[] GenerateQrCode(string email, byte[] key)
    {
        var base32Key = Base32Encoding.ToString(key);

        var escapedIssuer = Uri.EscapeDataString(
            tfaOptions.CurrentValue.Issuer);

        var escapedUser = Uri.EscapeDataString(email);

        var otpUri =
            $"otpauth://totp/{escapedIssuer}:{escapedUser}" +
            $"?secret={base32Key}" +
            $"&issuer={escapedIssuer}" +
            $"&digits={tfaOptions.CurrentValue.Digits}" +
            $"&period={tfaOptions.CurrentValue.Period}";

        using var qrGenerator = new QRCodeGenerator();

        using var qrCodeData = qrGenerator.CreateQrCode(
            otpUri,
            QRCodeGenerator.ECCLevel.Q);

        using var qrCode = new PngByteQRCode(qrCodeData);

        return qrCode.GetGraphic(10);
    }
    
    private byte[] GetEncryptionKey()
    {
        return Convert.FromBase64String(
            encryptionOptions.CurrentValue.Key);
    }
}