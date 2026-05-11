namespace Application.Models.Dtos;

public class MessageResponseDto
{
    public string Id { get; set; }
    public string ConversationId { get; set; }
    public string SenderUserId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}