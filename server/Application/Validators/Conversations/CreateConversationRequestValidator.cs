using Application.Models.Dtos.Conversations;
using FluentValidation;

namespace Application.Validators.Conversations;

public sealed class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequestDto>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.ListingId)
            .NotEmpty().WithMessage("ListingId is required.");
        
    }
    
}