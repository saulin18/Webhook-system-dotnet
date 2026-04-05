

using Webhooks.Processing.Domain.Users;

namespace Webhooks.Processing.Domain.Webhooks;

public sealed class WebhookDelivery : Entity
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public WebhookDeliveryStatus Status { get; set; } = WebhookDeliveryStatus.Pending;
    public int AttemptCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ErrorMessage { get; set; }
}
