using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Webhooks;
using SharedKernel;
using Application.Extensions;

namespace Application.Webhooks.GetAll;
internal sealed class GetAllSubscriptionsQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetAllSubscriptionsQuery, GetAllSubscriptionsResponseDto>
{
    public async Task<Result<GetAllSubscriptionsResponseDto>> Handle(GetAllSubscriptionsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<WebhookSubscription> querySubscriptions = context.WebhookSubscriptions.AsQueryable().Where(
            x => x.UserId == userContext.UserId
        );

        if(query.EventType is not null)
        {
            querySubscriptions = querySubscriptions.Where(x => x.EventType == query.EventType);
        }

        if(query.Url is not null)
        {
            querySubscriptions = querySubscriptions.Where(x => x.Url.Contains(query.Url));
        }

        if(query.IsActive is not null)
        {
            querySubscriptions = querySubscriptions.Where(x => x.IsActive == query.IsActive);
        }

        PagedResult<WebhookSubscription> result = await querySubscriptions.OrderBy(x => x.CreatedAt).
        ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        int totalPages = (int)Math.Ceiling(result.TotalCount / (decimal)query.PageSize);

        return new GetAllSubscriptionsResponseDto(result, totalPages);
    }
}