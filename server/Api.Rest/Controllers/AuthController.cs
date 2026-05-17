using Api.Rest.Extensions;
using Application.Interfaces;
using Application.Models.Dtos;
using Application.Models.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class AuthController(ISecurityService securityService) : ControllerBase
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
        return Ok(securityService.Login(dto));
    }

    [Route(RegisterRoute)]
    [HttpPost]
    public ActionResult<AuthResponseDto> Register([FromBody] RegisterRequestDto dto)
    {
        return Ok(securityService.Register(dto));
    }

    [HttpPost]
    [Route(SetupTfaRoute)]
    public ActionResult<TFASetupResponseDto> SetupTfa([FromHeader] string authorization) 
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        var responseDto = securityService.SetupTfa(jwt);
        return File(responseDto.QrCodeImage, "image/png");
    }

    [HttpPost]
    [Route(ValidateOtpRoute)]
    public ActionResult<ValidateOtpResponseDto> ValidateOtp([FromBody] ValidateOtpRequestDto dto,
        [FromHeader] string authorization)
    {
        securityService.VerifyJwtOrThrow(authorization);
        return Ok(securityService.ValidateTfa(dto));
    }
    
}