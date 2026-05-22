using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Users;
using Application.Services;
using Core.Domain.Entities;
using Moq;

namespace UnitTests.UserTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepo> _mockUserRepo = new();
    private readonly Mock<IPasswordService> _mockPasswordService = new();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_mockUserRepo.Object, _mockPasswordService.Object);
    }
    
    [Fact]
    public async Task UpdateUser_ValidUser_ReturnsUpdatedUser()
    {
        // Arrange
        var user = new User { Id = "1", Name = "Morten", Email = "morten@test.com" };

        _mockUserRepo.Setup(x => x.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUserRepo.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUserAsync(new UserUpdateRequestDto { Name = "Morten Updated" }, "1");

        // Assert
        Assert.Equal("Morten Updated", result.Name);
    }
    
    [Fact]
    public async Task UpdateUser_UpdatePassword_CallsHashPassword()
    {
        // Arrange
        var user = new User { Id = "1", Name = "Morten", Email = "morten@test.com", PasswordHash = "oldhash" };

        _mockUserRepo.Setup(x => x.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUserRepo.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.HashPassword("NewPassword!123")).Returns("newhash");

        // Act
        await _userService.UpdateUserAsync(new UserUpdateRequestDto { Password = "NewPassword!123" }, "1");

        // Assert — password change must go through hashing, never stored as plaintext
        _mockPasswordService.Verify(x => x.HashPassword("NewPassword!123"), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUser_ValidUser_ReturnsDeletedUser()
    {
        // Arrange
        var user = new User { Id = "1", Name = "Morten", Email = "morten@test.com" };

        _mockUserRepo.Setup(x => x.DeleteUserAsync("1")).ReturnsAsync(user);

        // Act
        var result = await _userService.DeleteUserAsync("1");

        // Assert
        Assert.Equal("1", result.Id);
        Assert.Equal("morten@test.com", result.Email);
        _mockUserRepo.Verify(x => x.DeleteUserAsync("1"), Times.Once);
    }
}