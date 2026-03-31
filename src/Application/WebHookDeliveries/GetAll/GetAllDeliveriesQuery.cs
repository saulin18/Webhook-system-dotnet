using Application.Abstractions.Messaging;
using Domain.Webhooks;

namespace Application.WebHookDeliveries.GetAll;

public sealed record GetAllDeliveriesQuery(Guid? subscriptionId, 
WebhookDeliveryStatus? status, string? cursor, int pageSize, string? eventType, 
DateTime? startDate, DateTime? endDate) : IQuery<GetAllDeliveriesResponseDto>;