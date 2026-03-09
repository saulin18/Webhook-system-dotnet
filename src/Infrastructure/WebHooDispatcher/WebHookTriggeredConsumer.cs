using System.Text;
using System.Text.Json;
using Domain.Webhooks;
using Infrastructure.Database;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.WebHookDispatcher;

internal sealed class WebHookTriggeredConsumer(
    IHttpClientFactory httpClientFactory,
    ApplicationDbContext dbContext,
    IConfiguration configuration)
    : IConsumer<WebHookTriggeredEvent>
{
    public async Task Consume(ConsumeContext<WebHookTriggeredEvent> context)
    {
        var message = context.Message;

        var client = httpClientFactory.CreateClient("Webhooks");

        // In test mode use the configured receiver URL so the test server can route the call in-process
        var webhookUrl = configuration.GetValue<bool>("Testing:Enabled")
            ? configuration["Testing:WebhookReceiverUrl"] ?? message.WebHookUrl
            : message.WebHookUrl;

        var payload = new
        {
            message.EventType,
            message.Payload,
            message.SubscriptionId,
            TimeStamp = DateTime.UtcNow,
            Id = Guid.NewGuid(),
        };

        var payloadJson = JsonSerializer.Serialize(payload);

        using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
        content.Headers.Add("X-Webhook-Secret", message.WebHookSecret);

        var delivery = new WebhookDelivery
        {
            Id = Guid.NewGuid(),
            SubscriptionId = message.SubscriptionId,
            EventType = message.EventType,
            Payload = payloadJson,
            Status = WebhookDeliveryStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            AttemptCount = 1,
        };

        try
        {
            var response = await client.PostAsync(webhookUrl, content);
            response.EnsureSuccessStatusCode();

            delivery.Status = WebhookDeliveryStatus.Complete;
            delivery.DeliveredAt = DateTime.UtcNow;

            await dbContext.WebhookDeliveries.AddAsync(delivery);
            await dbContext.SaveChangesAsync();

        }
        catch (Exception e)
        {

            delivery.Status = WebhookDeliveryStatus.Failed;
            delivery.ErrorMessage = e.Message;


            await dbContext.WebhookDeliveries.AddAsync(delivery);
            await dbContext.SaveChangesAsync();
        }
    }
}

