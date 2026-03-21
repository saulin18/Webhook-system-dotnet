

using Application.Extensions;
using Domain.Webhooks;
namespace Application.WebHookDeliveries;
public sealed record GetAllDeliveriesResponseDto(CursorPagedResult<WebhookDelivery> PagedResult, int TotalItems);