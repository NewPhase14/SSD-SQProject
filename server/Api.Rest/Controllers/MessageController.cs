using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class MessageController(IMessageService messageService) : ControllerBase
{
    public const string ControllerRoute = "/api/message";

    public const string SendMessageRoute = ControllerRoute + "/send";
    
    public const string GetMessagesRoute = ControllerRoute + "/messages";
    
    [HttpGet]
    [Route(GetMessagesRoute)]
    public async Task<IActionResult> GetMessages(string conversationId)
    {
        var messages = await messageService.GetMessagesAsync(conversationId);
        return Ok(messages);
    }

    [HttpPost]
    [Route(SendMessageRoute)]
    public async Task<IActionResult> SendMessage(string conversationId, string senderUserId, string plainText)
    {
        await messageService.SendMessageAsync(conversationId, senderUserId, plainText);
        return Ok();
    }

}        
