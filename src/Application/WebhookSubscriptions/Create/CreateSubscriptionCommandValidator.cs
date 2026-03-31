

using FluentValidation;

namespace Application.WebhookSubscriptions.Create;

internal sealed class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(c => c.Url).NotEmpty().Must(url => url.IsAbsoluteUri);
        RuleFor(c => c.EventType).NotEmpty();
    }
}