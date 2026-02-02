using Application.Abstractions.Messaging;

namespace Application.Webhooks.Delete;

public sealed record DeleteSubscriptionCommand(Guid Id) : ICommand<DeleteSubscriptionResponseDto>;