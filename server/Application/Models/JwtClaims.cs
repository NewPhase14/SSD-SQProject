namespace Application.Models;

public class JwtClaims
{
    public required string Email { get; set; }
    public required string Id { get; set; }
    public required string Exp { get; set; }
    public required string Type { get; set; }
}