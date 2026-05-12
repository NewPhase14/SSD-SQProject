using Application.Models.Dtos.Conversations;
using FluentValidation;

namespace Application.Validators.Conversations;

public sealed class ConversationCreateRequestValidator : AbstractValidator<ConversationCreateRequestDto>
{
    public ConversationCreateRequestValidator()
    {
        RuleFor(x => x.ListingId)
            .NotEmpty().WithMessage("ListingId is required.");
        
    }
    
}