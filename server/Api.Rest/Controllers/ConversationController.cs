using Application.Interfaces;
using Application.Models.Dtos.Conversations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class ConversationController(IConversationService conversationService, IJwtService jwtService) : ControllerBase
{
    private const string ControllerRoute = "/api/conversation";
    
    private const string CreateConversationRoute = ControllerRoute + "/Create";
    
    [HttpPost]
    [Route(CreateConversationRoute)]
    public async Task<ActionResult<ConversationResponseDto>> GetOrCreateConversations([FromBody]ConversationCreateRequestDto dto, [FromHeader] string authorization)
    {
        var jwt = jwtService.VerifyJwtOrThrow(authorization);
        if (jwt.Type != "Auth")
            return Unauthorized();
        
        return Ok(await conversationService.GetOrCreateConversationAsync(dto, jwt.Id));
    }
}