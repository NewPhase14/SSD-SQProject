namespace Application.Models.Dtos;

public class ListingResponseDto
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string? CategoryId { get; set; }
    public string Condition { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string Status { get; set; } = null!;
    public List<string> ImagePaths { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}