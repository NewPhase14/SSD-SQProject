using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.Postgres.Postgresql.Data;

public class ConversationRepo(MyDbContext ctx) : IConversationRepo
{
    public Task<Conversation?> GetAsync(string conversationId)
    {
        var conversation = ctx.Conversations.FirstOrDefault(c => c.Id == conversationId);
        return Task.FromResult(conversation);
    }

    public Task<Conversation?> GetByListingAndUsersAsync(string listingId, string buyerUserId, string sellerUserId)
    {
        var conversation = ctx.Conversations.FirstOrDefault(c =>
            c.ListingId == listingId &&
            c.BuyerUserId == buyerUserId &&
            c.SellerUserId == sellerUserId);

        return Task.FromResult(conversation);
    }

    public Task<Conversation> CreateAsync(Conversation conversation)
    {
        ctx.Conversations.Add(conversation);
        ctx.SaveChanges();
        return Task.FromResult(conversation);
    }
}