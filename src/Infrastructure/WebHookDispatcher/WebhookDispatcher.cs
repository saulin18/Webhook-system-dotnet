
using System.Linq;
using System.Net.Http;
using Domain.Webhooks;
using System.Text.Json;
using Infrastructure.Database;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Infrastructure.WebHookDispatcher;

internal sealed class WebhookDispatcher(IHttpClientFactory httpClientFactory, ApplicationDbContext context)
{

    public async Task DispatchAsync(string EventType, object payload)
    {
        List<WebhookSubscription> subscriptions = await context.WebhookSubscriptions
            .Where(x => x.EventType == EventType && x.IsActive)
            .ToListAsync();

        HttpClient client = httpClientFactory.CreateClient("Webhooks");

        foreach (WebhookSubscription subscription in subscriptions)
        {
            using var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");
            // When sending the webhook, you add the secret as a header

            content.Headers.Add("X-Webhook-Secret", subscription.Secret);

            HttpResponseMessage response = await client.PostAsync(subscription.Url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to dispatch webhook to {subscription.Url} with status code {response.StatusCode}");
            }
        }
    }
}

