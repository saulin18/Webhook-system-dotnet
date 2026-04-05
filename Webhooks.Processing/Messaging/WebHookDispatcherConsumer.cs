
using System.Diagnostics;
using MassTransit;
using Microsoft.EntityFrameworkCore;

using Webhooks.Processing.Database;
using Webhooks.Processing.Domain.Webhooks;

namespace Webhooks.Processing.Messaging;

internal sealed class WebHookDispatcherConsumer(ApplicationDbContext dbContext)
    : IConsumer<WebHookDispatcherMessage>
{
    public async Task Consume(ConsumeContext<WebHookDispatcherMessage> context)
    {
        WebHookDispatcherMessage message = context.Message;

        List<WebhookSubscription> subscriptions = await dbContext.WebhookSubscriptions
            .Where(x => x.EventType == message.EventType && x.IsActive)
            .ToListAsync();

        using Activity? activity = DiagnosticConfig.Source.StartActivity($"{message.EventType} Dispatching Webhooks");
        activity?.AddTag("webhook.subscriptions.count", subscriptions.Count);
        activity?.AddTag("EventType", (object)message.EventType);
        
        foreach (WebhookSubscription subscription in subscriptions)
        {
            await context.Publish(new WebHookTriggeredEvent(
                message.EventType, 
                subscription.Id,
                message.Payload, 
                subscription.Url, 
                subscription.Secret, 
                activity?.Id));
        }
    }
}