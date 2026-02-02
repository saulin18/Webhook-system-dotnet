using Domain.Users;

namespace Application.Webhooks.CreateSubscription;

public record CreateSubscriptionResponseDto(Guid SubscriptionId,
    DateTime CreatedAt, Guid UserId);
