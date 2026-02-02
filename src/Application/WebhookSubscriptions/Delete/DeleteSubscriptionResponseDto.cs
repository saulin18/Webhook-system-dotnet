using Application.Abstractions.Messaging;

namespace Application.Webhooks.Delete;

public sealed record DeleteSubscriptionResponseDto(Guid Id, DateTime DeletedAt);