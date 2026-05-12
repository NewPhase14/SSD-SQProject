using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;

public class ConversationRepo(MyDbContext ctx) : IConversationRepo
{
    public async Task<Conversation?> GetAsync(string conversationId)
    {
        var conversation = await ctx.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);
        return conversation;
    }

    public async Task<Conversation?> GetByListingAndBuyerAsync(string listingId, string buyerUserId)
    {
        var conversation = await ctx.Conversations.FirstOrDefaultAsync(c =>
            c.ListingId == listingId &&
            c.BuyerUserId == buyerUserId);

        return conversation;
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        await ctx.Conversations.AddAsync(conversation);
        await ctx.SaveChangesAsync();
        return conversation;
    }
}