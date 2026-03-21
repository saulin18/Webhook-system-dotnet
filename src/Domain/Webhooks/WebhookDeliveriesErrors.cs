using SharedKernel;

namespace Domain.Webhooks;

public static class WebhookDeliveriesErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("WebhookDeliveries.NotFound", $"Webhook delivery with ID {id} was not found.");

    public static Error Unauthorized() =>
        Error.Failure("WebhookDeliveries.Unauthorized", "You are not authorized to perform this action.");
}