using Application.Webhooks.Delete;
using FluentValidation;


namespace Application.Webhooks;


internal sealed class DeleteSubsciptionValidator : AbstractValidator<DeleteSubscriptionCommand>
{
    public DeleteSubsciptionValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
