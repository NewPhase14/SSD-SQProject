using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Users;

namespace Application.Services;

public class UserService(IUserRepo userRepo, IPasswordService passwordService) : IUserService
{
    public async Task<UserResponseDto> UpdateUserAsync(UserUpdateRequestDto dto, string userId)
    {
        var user = await userRepo.GetUserByIdAsync(userId);
        
        if (user is null)
            throw new InvalidOperationException("User not found");

        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = passwordService.HashPassword(dto.Password);

        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await userRepo.UpdateUserAsync(user);

        return new UserResponseDto
        {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Email = updatedUser.Email,
            CreatedAt = updatedUser.CreatedAt,
            UpdatedAt = updatedUser.UpdatedAt,
        };
    }

    public async Task<UserResponseDto> DeleteUserAsync(string userId)
    {
        var deletedUser = await userRepo.DeleteUserAsync(userId);
        if (deletedUser == null) throw new InvalidOperationException("User not found");

        return new UserResponseDto
        {
            Id = deletedUser.Id,
            Name = deletedUser.Name,
            Email = deletedUser.Email,
            CreatedAt = deletedUser.CreatedAt,
            UpdatedAt = deletedUser.UpdatedAt,
        };
    }
}