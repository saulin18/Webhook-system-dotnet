using Domain.Webhooks;

namespace Application.WebHookDeliveries.GetById;

public sealed record GetByIdResponseDto(Guid Id, Guid SubscriptionId, string EventType, 
string Payload, WebhookDeliveryStatus Status, DateTime CreatedAt, DateTime? DeliveredAt, string? ErrorMessage);