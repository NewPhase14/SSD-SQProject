using Application.Interfaces;
using Application.Models.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class AuthController(IAuthenticationService authenticationService ,IJwtService jwtService) : ControllerBase
{
    private const string ControllerRoute = "api/auth/";

    private const string LoginRoute = ControllerRoute + nameof(Login);

    private const string RegisterRoute = ControllerRoute + nameof(Register);

    private const string SetupTfaRoute = ControllerRoute + nameof(SetupTfa);
    
    private const string ValidateOtpRoute = ControllerRoute + nameof(ValidateOtp);

    [HttpPost]
    [Route(LoginRoute)]
    public ActionResult<AuthResponseDto> Login([FromBody] AuthRequestDto dto)
    {
        return Ok(authenticationService.Login(dto));
    }

    [Route(RegisterRoute)]
    [HttpPost]
    public ActionResult<AuthResponseDto> Register([FromBody] RegisterRequestDto dto)
    {
        return Ok(authenticationService.Register(dto));
    }

    [HttpPost]
    [Route(SetupTfaRoute)]
    public ActionResult<TfaSetupResponseDto> SetupTfa([FromHeader] string authorization) 
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type == "Auth")
        {
            var responseDto = authenticationService.SetupTfa(jwt);
            return File(responseDto.QrCodeImage, "image/png");
        }
        return Unauthorized();
    }

    [HttpPost]
    [Route(ValidateOtpRoute)]
    public ActionResult<AuthResponseDto> ValidateOtp([FromBody] ValidateOtpRequestDto dto,
        [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "2FA")
        {
            return Unauthorized();
        }
        return Ok(authenticationService.ValidateTfa(dto, jwt));
    }
    
}