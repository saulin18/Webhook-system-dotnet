using Application.Webhooks.Update;
using FluentValidation;

namespace Application.Webhooks.Update;

internal sealed class UpdateSubscriptionValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Url).NotEmpty().When(url => url is not null);
        RuleFor(c => c.EventType).NotEmpty().When(eventType => eventType is not null);
    }
}