using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Conversations;
using Core.Domain.Entities;

namespace Application.Services;

public class ConversationService(IConversationRepo conversationRepo, IListingRepo listingRepo) : IConversationService
{
    public async Task<ConversationResponseDto> GetOrCreateConversationAsync(ConversationCreateRequestDto dto, string userId)
    {
        var listingSellerId = await listingRepo.GetSellerIdAsync(dto.ListingId);
        
        if (listingSellerId == null)
            throw new Exception("Listing not found");
        
        if (listingSellerId == userId)
            throw new InvalidOperationException("Cannot create conversation with own listing");
        
        var existingConversation = await 
            conversationRepo.GetConversationByListingAndBuyerAsync(dto.ListingId, userId);

        if (existingConversation != null)
        {
            return new ConversationResponseDto()
            {
                Id = existingConversation.Id,
                ListingId = existingConversation.ListingId,
                BuyerUserId = existingConversation.BuyerUserId,
                SellerUserId = listingSellerId,
            };
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid().ToString(),
            ListingId = dto.ListingId,
            BuyerUserId = userId,
        };

        var newConversation = await conversationRepo.CreateConversationAsync(conversation);
        
        return new ConversationResponseDto()
        {
            Id = newConversation.Id,
            ListingId = newConversation.ListingId,
            BuyerUserId = newConversation.BuyerUserId,
            SellerUserId = listingSellerId,
        };
    }

    public async Task<List<ConversationResponseDto>> GetOwnConversationsAsync(string userId)
    {
        var conversations = await conversationRepo
            .GetAllConversationsByUserIdAsync(userId);
        
        if (conversations == null)
            throw new InvalidOperationException("Conversations not found");

        var result = new List<ConversationResponseDto>();
        
        foreach (var conversation in conversations)
        {
            var listingSellerId =
                await listingRepo.GetSellerIdAsync(conversation.ListingId);

            if (listingSellerId == null)
                throw new Exception("Listing not found");

            result.Add(new ConversationResponseDto
            {
                Id = conversation.Id,
                ListingId = conversation.ListingId,
                BuyerUserId = conversation.BuyerUserId,
                SellerUserId = listingSellerId
            });
        }
        return result;
    }
}