using Application.Abstractions.Messaging;
using Application.Abstractions.Data;
using Domain.Webhooks;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Authentication;

namespace Application.Webhooks.Delete;

internal sealed class DeleteSubscriptionHandler(
    IApplicationDbContext context, 
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<DeleteSubscriptionCommand, DeleteSubscriptionResponseDto>
{
    public async Task<Result<DeleteSubscriptionResponseDto>> Handle(DeleteSubscriptionCommand command, CancellationToken cancellationToken)
    {
        WebhookSubscription? subscription = await context.WebhookSubscriptions.SingleOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (subscription is null)
        {
            return Result.Failure<DeleteSubscriptionResponseDto>(WebhookSubscriptionErrors.NotFound(command.Id));
        }

        if (subscription.UserId != userContext.UserId)
        {
            return Result.Failure<DeleteSubscriptionResponseDto>(WebhookSubscriptionErrors.Unauthorized());
        }

        context.WebhookSubscriptions.Remove(subscription);
        await context.SaveChangesAsync(cancellationToken);

        return new DeleteSubscriptionResponseDto(subscription.Id, dateTimeProvider.UtcNow);
    }
}