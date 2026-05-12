using Application.Models.Dtos.Messages;

namespace Application.Interfaces;

public interface IMessageService
{
    public Task<MessageResponseDto> SendMessageAsync(MessageSendRequestDto dto, string userId);

    public Task<List<MessageResponseDto>> GetMessagesAsync(string conversationId, string userId);
    
}