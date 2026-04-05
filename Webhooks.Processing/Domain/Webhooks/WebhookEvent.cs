

namespace Webhooks.Processing.Domain.Webhooks;

public sealed class WebhookEvent(string eventType, string eventData)
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = eventType;
    public string EventData { get; set; } = eventData;
}
