using System.Diagnostics;
using System.Text;
using System.Text.Json;
using MassTransit;

using Webhooks.Processing.Domain.Webhooks;
using IApplicationDbContext = Webhooks.Processing.Database.IApplicationDbContext;

namespace Webhooks.Processing.Messaging;

internal sealed class WebHookTriggeredConsumer(
    IHttpClientFactory httpClientFactory,
    IApplicationDbContext dbContext,
    IConfiguration configuration)
    : IConsumer<WebHookTriggeredEvent>
{
    public async Task Consume(ConsumeContext<WebHookTriggeredEvent> context)
    {
        WebHookTriggeredEvent message = context.Message;
        string? parentActivityId = message.ActivityId;

        using Activity? activity = DiagnosticConfig.Source.StartActivity(
            "Processing Webhook Delivery", ActivityKind.Consumer, parentActivityId);
        activity?.AddTag("EventType", (object)message.EventType);

        HttpClient client = httpClientFactory.CreateClient("Webhooks");

        string webhookUrl = configuration.GetValue<bool>("Testing:Enabled")
            ? configuration["Testing:WebhookReceiverUrl"] ?? message.WebHookUrl
            : message.WebHookUrl;

        object payload = new
        {
            message.EventType,
            message.Payload,
            message.SubscriptionId,
            TimeStamp = DateTime.UtcNow,
            Id = Guid.NewGuid(),
        };

        string payloadJson = JsonSerializer.Serialize(payload);

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
            HttpResponseMessage response = await client.PostAsync(webhookUrl, content);
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

