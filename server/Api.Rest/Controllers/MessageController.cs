using Application.Interfaces;
using Application.Models.Dtos.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class MessageController(IMessageService messageService, IJwtService jwtService) : ControllerBase
{
    private const string ControllerRoute = "/api/message";

    private const string SendMessageRoute = ControllerRoute + "/send";
    
    private const string GetMessagesRoute = ControllerRoute + "/messages";
    
    [HttpGet]
    [Route(GetMessagesRoute)]
    public async Task<IActionResult> GetMessages(string conversationId, [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "Auth")
            return Unauthorized();
        
        return Ok(await messageService.GetMessagesAsync(conversationId, jwt.Id));
    }

    [HttpPost]
    [Route(SendMessageRoute)]
    public async Task<ActionResult<MessageResponseDto>> SendMessage([FromBody]MessageSendRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "Auth")
            return Unauthorized();

        return Ok(await messageService.SendMessageAsync(dto, jwt.Id));
    }

}        
