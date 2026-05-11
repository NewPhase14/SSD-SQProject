using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;

namespace Application.Models.Dtos;

public class ListingCreateRequestDto
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required] 
    public string CategoryId { get; set; } = null!;

    [Required]
    public string Condition { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Status { get; set; } = null!;

    [Required] 
    public List<string> ImagePaths { get; set; } = null!;
}