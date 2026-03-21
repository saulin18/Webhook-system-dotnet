
using Domain.Webhooks;
using Application.Abstractions.Messaging;

namespace Application.WebHookDeliveries;

public sealed record GetAllDeliveriesQuery(Guid? subscriptionId, 
WebhookDeliveryStatus? status, string? cursor, int pageSize, string? eventType, 
DateTime? startDate, DateTime? endDate) : IQuery<GetAllDeliveriesResponseDto>;