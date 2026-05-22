using Application.Models;
using Application.Models.Dtos.Auth;

namespace Application.Interfaces;

public interface IAuthenticationService
{
    public Task<AuthResponseDto> Register(RegisterRequestDto dto);
    public Task<AuthResponseDto> Login(AuthRequestDto dto);
    public Task<TfaSetupResponseDto> SetupTfa(JwtClaims jwt);
    public Task<AuthResponseDto> ValidateTfa(ValidateOtpRequestDto dto, JwtClaims jwt);
  
}