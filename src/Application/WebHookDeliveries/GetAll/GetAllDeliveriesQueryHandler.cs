using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Extensions;
using Domain.Webhooks;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.WebHookDeliveries;

internal sealed class GetAllDeliveriesQueryHandler(IApplicationDbContext context, IUserContext userContext) :
IQueryHandler<GetAllDeliveriesQuery, GetAllDeliveriesResponseDto>
{
    public async Task<Result<GetAllDeliveriesResponseDto>> Handle(GetAllDeliveriesQuery query,
    CancellationToken cancellationToken)
    {

        var ownedSubscriptionIds = context.WebhookSubscriptions
            .Where(x => x.UserId == userContext.UserId)
            .Select(x => x.Id);

        IQueryable<WebhookDelivery> queryDeliveries = context.WebhookDeliveries.Where(
            x => ownedSubscriptionIds.Contains(x.SubscriptionId));

        if (query.status is not null)
        {
            queryDeliveries = queryDeliveries.Where(x => x.Status == query.status);
        }

        if (query.eventType is not null)
        {
            queryDeliveries = queryDeliveries.Where(x => x.EventType == query.eventType);
        }

        if (query.subscriptionId is not null)
        {
            queryDeliveries = queryDeliveries.Where(x => x.SubscriptionId == query.subscriptionId);
        }

        if (query.startDate is not null)
        {
            queryDeliveries = queryDeliveries.Where(x => x.CreatedAt >= query.startDate);
        }

        if (query.endDate is not null)
        {
            queryDeliveries = queryDeliveries.Where(x => x.CreatedAt <= query.endDate);
        }

        var totalCount = await queryDeliveries.CountAsync(cancellationToken);

        var pagedResult = await queryDeliveries.ToCursorPagedResultAsync<WebhookDelivery, Guid>
        (x => x.Id, query.cursor, query.pageSize, PaginationDirection.Forward, cancellationToken);

        return Result.Success(new GetAllDeliveriesResponseDto(pagedResult, totalCount));
    }
}