using Application.Models.Dtos.Conversations;
using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IConversationService
{
    Task<ConversationResponseDto> GetOrCreateConversationAsync(CreateConversationRequestDto dto, string userId);
}