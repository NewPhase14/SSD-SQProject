using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;

public class MessageRepo(MyDbContext ctx) : IMessageRepo
{
    
    public async Task<Message> AddMessageAsync(Message message)
    {
        await ctx.Messages.AddAsync(message);
        await ctx.SaveChangesAsync();
        return message;
    }

    public async Task<List<Message>> GetByConversationIdAsync(string conversationId)
    {
        var messages = await ctx.Messages.Where(m => m.ConversationId == conversationId).OrderByDescending(m => m.CreatedAt).ToListAsync();
        return messages;
    }
}