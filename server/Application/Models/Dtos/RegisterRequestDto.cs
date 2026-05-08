using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public class RegisterRequestDto
{
    [MinLength(3)] [Required] public string Email { get; set; } = null!;
    [MinLength(4)] [Required] public string Password { get; set; } = null!;
    [MinLength(2)] [Required] public string Name { get; set; } = null!;
}