using Application.Models.Dtos.Auth;
using Application.Validators.Auth;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Password_ValidPassword_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Empty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert — empty password triggers the minimum length rule first
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Password_TooShort_ShouldHaveValidationError()
    {
        // Arrange — 6 characters, below the 8 character minimum
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "P!1abc"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Password_MissingUppercase_ShouldHaveValidationError()
    {
        // Arrange — all lowercase letters
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "password!123"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Password_MissingLowercase_ShouldHaveValidationError()
    {
        // Arrange — all uppercase letters
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "PASSWORD!123"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Password_MissingNumber_ShouldHaveValidationError()
    {
        // Arrange — special characters present but no digits
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!!!"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one number.");
    }

    [Fact]
    public void Password_MissingSpecialCharacter_ShouldHaveValidationError()
    {
        // Arrange — letters and numbers but no special character
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character.");
    }
}