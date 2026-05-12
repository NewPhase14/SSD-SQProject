using Application.Models.Dtos.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class AuthRequestValidator : AbstractValidator<AuthRequestDto>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}