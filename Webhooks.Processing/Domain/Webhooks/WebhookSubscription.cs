

using Webhooks.Processing.Domain.Users;

namespace Webhooks.Processing.Domain.Webhooks;

public sealed class WebhookSubscription : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
