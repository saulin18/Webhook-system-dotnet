using SharedKernel;

namespace Domain.Webhooks;

public static class WebhookSubscriptionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("WebhookSubscription.NotFound", $"Webhook subscription with ID {id} was not found.");

    public static Error InvalidUrl(Uri url) =>
        Error.Problem("WebhookSubscription.InvalidUrl", $"Invalid webhook URL: {url}");

    public static Error DuplicateUrl(Uri url) =>
        Error.Conflict("WebhookSubscription.DuplicateUrl", $"Webhook subscription with URL {url} already exists.");

    public static Error Unauthorized() =>
        Error.Failure("WebhookSubscription.Unauthorized", "You are not authorized to perform this action.");
}