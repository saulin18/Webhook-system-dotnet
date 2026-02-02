using Application.Abstractions.Messaging;
using Application.Abstractions.Data;
using Domain.Webhooks;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Authentication;

namespace Application.Webhooks.Update;

internal sealed class UpdateSubscriptionHandler(
    IApplicationDbContext context, 
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateSubscriptionCommand, UpdateSubscriptionResponseDto>
{
    public async Task<Result<UpdateSubscriptionResponseDto>> Handle(UpdateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        WebhookSubscription? subscription = await context.WebhookSubscriptions.SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (subscription is null)
        {
            return Result.Failure<UpdateSubscriptionResponseDto>(WebhookSubscriptionErrors.NotFound(command.Id));
        }

        if(subscription.UserId != userContext.UserId)
        {
            return Result.Failure<UpdateSubscriptionResponseDto>(WebhookSubscriptionErrors.Unauthorized());
        }

        if (command.Url is not null)
        {
            subscription.Url = command.Url.ToString();
        }

        if (command.EventType is not null)
        {
            subscription.EventType = command.EventType;
        }

        if (command.IsActive is not null)
        {
            subscription.IsActive = command.IsActive.Value;
        }

        DateTime utcNow = dateTimeProvider.UtcNow;
        subscription.UpdatedAt = utcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateSubscriptionResponseDto(subscription.Id, utcNow);
    }
}