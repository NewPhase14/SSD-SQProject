using Application.Models.Dtos.Users;

namespace Application.Interfaces;

public interface IUserService
{
    public Task<UserResponseDto> GetUserByEmailAsync(string email);
    public Task<UserResponseDto> UpdateUserAsync(UserUpdateRequestDto dto, string userId);
    public Task<UserResponseDto> DeleteUserAsync(string userId);
}