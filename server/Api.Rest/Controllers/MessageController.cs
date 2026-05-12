using Application.Interfaces;
using Application.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class MessageController(IMessageService messageService, ISecurityService securityService) : ControllerBase
{
    private const string ControllerRoute = "/api/message";

    private const string SendMessageRoute = ControllerRoute + "/send";
    
    private const string GetMessagesRoute = ControllerRoute + "/messages";
    
    [HttpGet]
    [Route(GetMessagesRoute)]
    public async Task<IActionResult> GetMessages(string conversationId, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        var messages = await messageService.GetMessagesAsync(conversationId, jwt.Id);
        return Ok(messages);
    }

    [HttpPost]
    [Route(SendMessageRoute)]
    public async Task<IActionResult> SendMessage([FromBody]SendMessageRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        await messageService.SendMessageAsync(dto, jwt.Id);
        return Ok();
    }

}        
