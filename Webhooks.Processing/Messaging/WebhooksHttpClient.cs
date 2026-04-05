
namespace Webhooks.Processing.Messaging;

public sealed class WebhooksHttpClient(HttpClient http)
{
    public Task<HttpResponseMessage> PostWebhookAsync(string url, HttpContent content, CancellationToken ct = default)
        => http.PostAsync(url, content, ct);

    public Task<HttpResponseMessage> PostWebhookAsync(Uri url, HttpContent content, CancellationToken ct = default)
    {
        return http.PostAsync(url.ToString(), content, ct);
    }
}