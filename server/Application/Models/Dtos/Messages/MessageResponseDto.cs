namespace Application.Models.Dtos.Messages;

public class MessageResponseDto
{
    public string Id { get; set; } = null!;
    public string ConversationId { get; set; } = null!;
    public string SenderUserId { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
}