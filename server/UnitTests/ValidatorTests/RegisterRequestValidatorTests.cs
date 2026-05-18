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
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Empty_ShouldHaveValidationError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = ""
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Password_TooShort_ShouldHaveValidationError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "P!1abc"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Password_MissingUppercase_ShouldHaveValidationError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "password!123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Password_MissingLowercase_ShouldHaveValidationError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "PASSWORD!123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Password_MissingNumber_ShouldHaveValidationError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password!!!"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one number.");
    }

    [Fact]
    public void Password_MissingSpecialCharacter_ShouldHaveValidationError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Morten",
            Email = "morten@test.com",
            Password = "Password123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character.");
    }
}