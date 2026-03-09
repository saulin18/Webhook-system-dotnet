using Domain.Webhooks;
using Infrastructure.WebHookDispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebhookTests.Integration;
using Xunit;

namespace WebhookTests.Integration.Deliveries;

public sealed class DispatchTest : BaseIntegrationTest
{
    public DispatchTest(WebhookIntegrationFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Dispatch_WithValidData_ReturnsSuccess()
    {
        var requestBody = new
        {
            EventType = "test",
            payload = new { UserId = Guid.NewGuid(), UserName = "testuser" }
        };

       
        var exampleSubscriptions = SeedingUtils.GetSeedingWebhookSubscriptions(Guid.NewGuid());
        var subscription = exampleSubscriptions[0];
        subscription.Url = CustomWebApplicationFactory.TestWebhookReceiverUrl;
        await SeedingUtils.SeedSubscription(DbContext, subscription);

        var dispatcher = Scope.ServiceProvider.GetRequiredService<WebHookDispatcherMassTransit>();
        await dispatcher.DispatchAsync(requestBody.EventType, requestBody.payload);

        // Give the bus and consumers time to process (async message → dispatcher → triggered consumer → HTTP → save)
        await Task.Delay(TimeSpan.FromSeconds(2));

        WebhookDelivery? delivery = null;
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (DateTime.UtcNow < deadline)
        {
            delivery = await DbContext.WebhookDeliveries
                .FirstOrDefaultAsync(d => d.EventType == requestBody.EventType);
            if (delivery is not null)
                break;
            await Task.Delay(200);
        }


        Assert.Contains("testuser", delivery.Payload, StringComparison.Ordinal);
        Assert.Equal(WebhookDeliveryStatus.Complete, delivery.Status);
        Assert.Equal(requestBody.EventType, delivery.EventType);
    }
}