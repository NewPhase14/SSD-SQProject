using Application.Models;
using Application.Models.Dtos.Auth;

namespace Application.Interfaces;

public interface IAuthenticationService
{
    public AuthResponseDto Register(RegisterRequestDto dto);
    public AuthResponseDto Login(AuthRequestDto dto);
    public TfaSetupResponseDto SetupTfa(JwtClaims jwt);
    public AuthResponseDto ValidateTfa(ValidateOtpRequestDto dto, JwtClaims jwt);
  
}