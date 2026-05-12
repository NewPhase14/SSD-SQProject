using Microsoft.AspNetCore.Http;

namespace Application.Models.Dtos.Listings;

public class ListingCreateRequestDto
{
    public string CategoryId { get; set; } = null!;

    public string Condition { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public List<IFormFile> Images { get; set; } = new();

}