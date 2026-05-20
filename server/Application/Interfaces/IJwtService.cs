using Application.Models;

namespace Application.Interfaces;

public interface IJwtService
{
    public string GenerateJwt(JwtClaims claims);
    public JwtClaims VerifyJwtOrThrow(string jwt);
}
