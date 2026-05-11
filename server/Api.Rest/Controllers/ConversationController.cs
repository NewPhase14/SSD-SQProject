using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class ConversationController(IConversationService conversationService) : ControllerBase
{
    public const string ControllerRoute = "/api/conversation";
    
    public const string CreateConversationRoute = ControllerRoute + "/Create";
    

    
    [HttpPost]
    [Route(CreateConversationRoute)]
    public async Task<IActionResult> GetOrCreateConversations(string listingId, string buyerUserId, string sellerUserId)
    {
        var conversation = await conversationService.GetOrCreateConversationAsync(listingId, buyerUserId, sellerUserId);
        return Ok(conversation);
    }
   
    
    
    
}