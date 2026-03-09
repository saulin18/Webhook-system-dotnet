using Web.Api.Endpoints;

namespace Web.Api.Endpoints.Test;

/// <summary>
/// Minimal POST endpoint that returns 200. Used so integration tests can send webhooks
/// to the same server (e.g. Dispatch test) and get a successful response.
/// </summary>
internal sealed class WebhookReceiver : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("test/webhook-receiver", () => Results.Ok())
            .WithTags(Tags.Webhooks);
    }
}
