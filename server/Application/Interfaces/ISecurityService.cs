using Application.Models;
using Application.Models.Dtos.Auth;

namespace Application.Interfaces;

public interface ISecurityService
{
    public string HashPassword(string password);
    
    public void VerifyPasswordOrThrow(string password, string hashedPassword);
    
    public string GenerateSalt();
    
    public string GenerateJwt(JwtClaims claims);
    
    public AuthResponseDto Login(AuthRequestDto dto);
    
    public TfaSetupResponseDto SetupTfa(JwtClaims jwt);
    
    public AuthResponseDto ValidateTfa(ValidateOtpRequestDto dto, JwtClaims jwt);
    
    public AuthResponseDto Register(RegisterRequestDto dto);
    
    public JwtClaims VerifyJwtOrThrow(string jwt);
}