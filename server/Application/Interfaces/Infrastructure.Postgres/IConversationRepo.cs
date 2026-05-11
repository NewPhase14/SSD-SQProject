using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IConversationRepo
{
    Task<Conversation?> GetAsync(string conversationId);

    Task<Conversation?> GetByListingAndUsersAsync(
        string listingId,
        string buyerUserId,
        string sellerUserId);

    Task<Conversation> CreateAsync(Conversation conversation);
}
