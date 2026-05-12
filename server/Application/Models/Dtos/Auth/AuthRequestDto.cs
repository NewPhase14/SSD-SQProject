namespace Application.Models.Dtos.Auth;

public class AuthRequestDto
{
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
}