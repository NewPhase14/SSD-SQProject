using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IConversationRepo
{
    Task<Conversation?> GetConversationAsync(string conversationId);

    Task<Conversation?> GetConversationByListingAndBuyerAsync(
        string listingId,
        string buyerUserId);

    Task<Conversation> CreateConversationAsync(Conversation conversation);
    
    Task<List<Conversation>> GetAllConversationsByUserIdAsync(string userId);
}
