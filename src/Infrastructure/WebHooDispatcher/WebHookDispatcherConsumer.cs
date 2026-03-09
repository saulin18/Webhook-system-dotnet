using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Domain.Webhooks;
using Infrastructure.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.WebHookDispatcher;

internal sealed record WebHookDispatcherMessage(string EventType, object Payload);

internal sealed class WebHookDispatcherConsumer(ApplicationDbContext dbContext
) : IConsumer<WebHookDispatcherMessage>
{
    public async Task Consume(ConsumeContext<WebHookDispatcherMessage> context)
    {
        var Message = context.Message;

        var subscriptions = await dbContext.WebhookSubscriptions
            .Where(x => x.EventType == Message.EventType && x.IsActive)
            .ToListAsync();

        foreach (var subscription in subscriptions)
        {
            await context.Publish(new WebHookTriggeredEvent(Message.EventType, subscription.Id,
            Message.Payload, subscription.Url, subscription.Secret));

            //  await context.PublishBatch( subscriptions.Select(subscription 
            //  => new WebHookTriggeredEvent(Message.EventType, subscription.Id,
            //  Message.Payload, subscription.Url)));
        }
    }
}

internal sealed record WebHookTriggeredEvent(string EventType, Guid SubscriptionId, object Payload,
    string WebHookUrl, string WebHookSecret);