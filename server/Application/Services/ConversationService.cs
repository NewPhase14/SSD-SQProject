using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;

namespace Application.Services;

public class ConversationService(IConversationRepo conversationRepo) : IConversationService
{
    public async Task<Conversation> GetOrCreateConversationAsync(string listingId, string buyerUserId,
        string sellerUserId)
    {
        var existingConversation =
            conversationRepo.GetByListingAndUsersAsync(listingId, buyerUserId, sellerUserId).Result;

        if (existingConversation != null)
        {
            return existingConversation;
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid().ToString(),
            ListingId = listingId,
            BuyerUserId = buyerUserId,
            SellerUserId = sellerUserId,
        };

        return await conversationRepo.CreateAsync(conversation);
    }
}