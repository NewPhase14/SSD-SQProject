using Application.Models.Dtos.Messages;
using FluentValidation;

namespace Application.Validators.Messages;

public sealed class MessageSendRequestValidator : AbstractValidator<MessageSendRequestDto>
{
    public MessageSendRequestValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("ConversationId is required.");
        
        RuleFor(x => x.PlainText)
            .NotEmpty().WithMessage("PlainText is required.");
    }
    
}