using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IMessageService
{
    public Task SendMessageAsync(SendMessageRequestDto dto, string userId);

    public Task<List<MessageResponseDto>> GetMessagesAsync(string conversationId, string userId);
    
}