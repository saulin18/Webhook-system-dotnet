using Application.Abstractions.Messaging;
using Application.Abstractions.Data;
using Domain.Webhooks;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Authentication;

namespace Application.Webhooks.GetById;

internal sealed class GetByIdQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetByIdQuery, GetByIdResponseDto>
{
    public async Task<Result<GetByIdResponseDto>> Handle(GetByIdQuery query, CancellationToken cancellationToken)
    {
        WebhookSubscription? subscription = await context.WebhookSubscriptions.
        SingleOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (subscription is null)
        {
            return Result.Failure<GetByIdResponseDto>(WebhookSubscriptionErrors.NotFound(query.Id));
        }

        if (subscription.UserId != userContext.UserId)
        {
            return Result.Failure<GetByIdResponseDto>(WebhookSubscriptionErrors.Unauthorized());
        }

        return new GetByIdResponseDto(subscription.Id, subscription.Url, subscription.EventType, subscription.IsActive, subscription.CreatedAt);
    }
}