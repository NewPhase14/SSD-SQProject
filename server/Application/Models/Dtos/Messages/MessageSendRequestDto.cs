namespace Application.Models.Dtos.Messages;

public class MessageSendRequestDto
{
    public string ConversationId { get; set; } = null!;

    public string PlainText { get; set; } = null!;
}