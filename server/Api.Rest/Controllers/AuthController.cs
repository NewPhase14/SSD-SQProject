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
}