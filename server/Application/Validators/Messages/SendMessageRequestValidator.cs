using Application.Models.Dtos;
using FluentValidation;

namespace Application.Validators.Messages;

public sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequestDto>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("ConversationId is required.");
        
        RuleFor(x => x.PlainText)
            .NotEmpty().WithMessage("PlainText is required.");
    }
    
}