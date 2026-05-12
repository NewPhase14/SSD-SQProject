namespace Application.Models.Dtos.Conversations;

public class ConversationResponseDto
{
    public string Id { get; set; } = null!;

    public string ListingId { get; set; } = null!;

    public string BuyerUserId { get; set; } = null!;
    
    public string SellerUserId { get; set; } = null!;

}