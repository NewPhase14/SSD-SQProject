namespace Application.Models.Dtos;

public class ListingUpdateRequestDto
{
    public string Id { get; set; }
    public string CategoryId { get; set; }
    public string Condition { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    
}