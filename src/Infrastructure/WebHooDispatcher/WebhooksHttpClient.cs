using System.Net.Http;
namespace Infrastructure.WebHooDispatcher;

public sealed class WebhooksHttpClient
{
    private readonly HttpClient http;
    public WebhooksHttpClient(HttpClient http) => this.http = http;

    public Task<HttpResponseMessage> PostWebhookAsync(string url, HttpContent content, CancellationToken ct = default)
        => http.PostAsync(url, content, ct);

    public Task<HttpResponseMessage> PostWebhookAsync(Uri url, HttpContent content, CancellationToken ct = default)
    {
        return http.PostAsync(url.ToString(), content, ct);
    }
}