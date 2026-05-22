using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;

public class ConversationRepo(MyDbContext ctx) : IConversationRepo
{
    public async Task<Conversation?> GetConversationAsync(string conversationId)
    {
        var conversation = await ctx.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);
        return conversation;
    }

    public async Task<Conversation?> GetConversationByListingAndBuyerAsync(string listingId, string buyerUserId)
    {
        var conversation = await ctx.Conversations.FirstOrDefaultAsync(c =>
            c.ListingId == listingId &&
            c.BuyerUserId == buyerUserId);

        return conversation;
    }

    public async Task<Conversation> CreateConversationAsync(Conversation conversation)
    {
        await ctx.Conversations.AddAsync(conversation);
        await ctx.SaveChangesAsync();
        return conversation;
    }

    public async Task<List<Conversation>> GetAllConversationsByUserIdAsync(string userId)
    {
        var conversations = await ctx.Conversations.Where(c => c.BuyerUserId == userId).ToListAsync();
        return conversations;
    }
}