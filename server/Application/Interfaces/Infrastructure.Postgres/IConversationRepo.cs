using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IConversationRepo
{
    Task<Conversation?> GetAsync(string conversationId);

    Task<Conversation?> GetByListingAndBuyerAsync(
        string listingId,
        string buyerUserId);

    Task<Conversation> CreateAsync(Conversation conversation);
}
