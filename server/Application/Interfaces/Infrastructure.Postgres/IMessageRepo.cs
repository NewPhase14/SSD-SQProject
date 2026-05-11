using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IMessageRepo
{
    Task<Message> AddMessageAsync(Message message);

    Task<List<Message>> GetByConversationIdAsync(string conversationId);
}