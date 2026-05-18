namespace Application.Models.Dtos.Auth;

public class AuthResponseDto
{
    public bool TfaIsRequired { get; set; }
    public string Jwt { get; set; } = null!;
}