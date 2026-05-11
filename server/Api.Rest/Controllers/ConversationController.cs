using Application.Interfaces;
using Application.Models.Dtos.Conversations;
using Application.Validators.Conversations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class ConversationController(IConversationService conversationService, ISecurityService securityService) : ControllerBase
{
    private const string ControllerRoute = "/api/conversation";
    
    private const string CreateConversationRoute = ControllerRoute + "/Create";
    
    
    [HttpPost]
    [Route(CreateConversationRoute)]
    public async Task<ActionResult<ConversationResponseDto>> GetOrCreateConversations([FromBody]CreateConversationRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        var conversation = await conversationService.GetOrCreateConversationAsync(dto, jwt.Id);
        return Ok(conversation);
    }
   
    
    
    
}