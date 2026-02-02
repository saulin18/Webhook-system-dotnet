using SharedKernel;

namespace Domain.Webhooks;

public sealed class WebhookEvent
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public string EventData { get; set; }

    public WebhookEvent(string eventType, string eventData)
    {
        EventType = eventType;
        EventData = eventData;
    }
}

