using Application.Extensions;
using Domain.Webhooks;
using SharedKernel;

namespace Application.Webhooks.GetAll;
public sealed record GetAllSubscriptionsResponseDto(PagedResult<WebhookSubscription> PagedResult, int TotalPages);