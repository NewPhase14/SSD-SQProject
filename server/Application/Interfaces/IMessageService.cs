using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IMessageService
{
    public Task SendMessageAsync(
        string conversationId,
        string senderUserId,
        string plainText);

    public Task<List<MessageResponseDto>> GetMessagesAsync(string conversationId);
    
}