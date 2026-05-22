using Application.Models.Dtos.Users;

namespace Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> UpdateUserAsync(UserUpdateRequestDto dto, string userId);
    Task<UserResponseDto> DeleteUserAsync(string userId);
}