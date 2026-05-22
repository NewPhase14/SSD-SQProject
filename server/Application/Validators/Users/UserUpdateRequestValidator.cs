using Application.Models.Dtos.Users;
using FluentValidation;

namespace Application.Validators.Users;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequestDto>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
        
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~]").WithMessage(
                "Password must contain at least one special character.")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}