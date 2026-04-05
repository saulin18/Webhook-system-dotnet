namespace Webhooks.Processing.Messaging;

internal sealed record WebHookDispatcherMessage(string EventType, object Payload);

internal sealed record WebHookTriggeredEvent(
    string EventType, 
    Guid SubscriptionId, 
    object Payload,
    string WebHookUrl, 
    string WebHookSecret, 
    string? ActivityId);
