using Application.Interfaces;
using Application.Models.Dtos.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class UserController(IUserService userService, IJwtService jwtService) : ControllerBase
{
    private const string ControllerRoute = "api/user/";
    
    private const string UpdateRoute = ControllerRoute + nameof(Update);

    private const string DeleteRoute = ControllerRoute + nameof(Delete);

    private const string GetUserByEmailRoute = ControllerRoute + nameof(GetUserByEmail);
    
    
    [HttpGet]
    [Route(GetUserByEmailRoute)]
    public async Task<ActionResult<UserResponseDto>> GetUserByEmail([FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "Auth")
            return Unauthorized();
        
        return Ok(await userService.GetUserByEmailAsync(jwt.Email));
    }
    
    [HttpPut]
    [Route(UpdateRoute)]
    public async Task<ActionResult<UserResponseDto>> Update([FromBody] UserUpdateRequestDto dto,
        [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "Auth")
            return Unauthorized();
        
        return Ok(await userService.UpdateUserAsync(dto, jwt.Id));
    }
    
    [HttpDelete]
    [Route(DeleteRoute)]
    public async Task<ActionResult<UserResponseDto>> Delete([FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "Auth")
            return Unauthorized();

        return Ok(await userService.DeleteUserAsync(jwt.Id));
    }
}