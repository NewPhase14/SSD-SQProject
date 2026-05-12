namespace Application.Models.Dtos.Auth;

public class RegisterRequestDto
{
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public string Name { get; set; } = null!;
}