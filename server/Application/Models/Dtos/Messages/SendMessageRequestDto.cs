namespace Application.Models.Dtos;

public class SendMessageRequestDto
{
    public string ConversationId { get; set; } = null!;

    public string PlainText { get; set; } = null!;
}