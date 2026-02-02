using Application.Abstractions.Messaging;

namespace Application.Webhooks;

public sealed record UpdateSubscriptionResponseDto(Guid Id, DateTime UpdatedAt);