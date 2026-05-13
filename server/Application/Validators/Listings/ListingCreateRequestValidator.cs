using Application.Models.Dtos.Listings;
using FluentValidation;

namespace Application.Validators.Listings;

public class ListingCreateRequestValidator : AbstractValidator<ListingCreateRequestDto>
{
    
    public ListingCreateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(50).WithMessage("Title must not exceed 50 characters.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.Condition)
            .NotEmpty().WithMessage("Condition is required");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required");
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image is required.")
            .Must(images => images.Count <= 10)
            .WithMessage("Maximum 10 images allowed.");
    }
}