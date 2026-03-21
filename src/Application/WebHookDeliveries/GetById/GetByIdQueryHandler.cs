using Application.Abstractions.Data;
using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Webhooks;
using SharedKernel;
using Application.WebHookDeliveries.GetById;
using Microsoft.EntityFrameworkCore;

namespace Application.WebHookDeliveries.GetById;

internal sealed class GetByIdQueryHandler(IApplicationDbContext context, IUserContext userContext) :
IQueryHandler<GetByIdQuery, GetByIdResponseDto>
{
    public async Task<Result<GetByIdResponseDto>> Handle(GetByIdQuery query, CancellationToken cancellationToken)
    {
        var delivery = await context.WebhookDeliveries.FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);
        if (delivery is null)
        {
            return Result.Failure<GetByIdResponseDto>(WebhookDeliveriesErrors.NotFound(query.Id));
        }

        var webhookSubscription = await context.WebhookSubscriptions.FirstOrDefaultAsync(x => x.Id == delivery.SubscriptionId, cancellationToken);

        if (webhookSubscription is null)
        {
            return Result.Failure<GetByIdResponseDto>(WebhookDeliveriesErrors.NotFound(delivery.SubscriptionId));
        }

        if (webhookSubscription.UserId != userContext.UserId)
        {
            return Result.Failure<GetByIdResponseDto>(WebhookDeliveriesErrors.Unauthorized());
        }

        return new GetByIdResponseDto(delivery.Id, delivery.SubscriptionId, delivery.EventType, delivery.Payload, delivery.Status, delivery.CreatedAt, delivery.DeliveredAt, delivery.ErrorMessage);
    }
}