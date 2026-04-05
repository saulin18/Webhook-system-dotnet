
using MassTransit;

namespace Infrastructure.WebHookDispatcher;

internal sealed record WebHookDispatcherMessage(string EventType, object Payload);

public sealed class WebHookDispatcherMassTransit(
    IPublishEndpoint endpoint
)
{
    public async Task DispatchAsync<T>(string eventType, T payload) where T : notnull
    {
        await endpoint.Publish(new WebHookDispatcherMessage(eventType, payload));
    }
}