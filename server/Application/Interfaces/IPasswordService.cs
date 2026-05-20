namespace Application.Interfaces;

public abstract class IPasswordService
{
    public abstract string HashPassword(string password);
    public abstract void VerifyPasswordOrThrow(string password, string storedHash);
}