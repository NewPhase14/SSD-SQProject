using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IConversationService
{
    Task<Conversation> GetOrCreateConversationAsync(
        string listingId,
        string buyerUserId,
        string sellerUserId);
}