using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.Postgres.Postgresql.Data;

public class MessageRepo(MyDbContext ctx) : IMessageRepo
{
    
    public Task<Message> AddMessageAsync(Message message)
    {
        ctx.Messages.Add(message);
        ctx.SaveChanges();
        return Task.FromResult(message);
    }

    public Task<List<Message>> GetByConversationIdAsync(string conversationId)
    {
        var messages = ctx.Messages.Where(m => m.ConversationId == conversationId).ToList();
        return Task.FromResult(messages);
    }
}