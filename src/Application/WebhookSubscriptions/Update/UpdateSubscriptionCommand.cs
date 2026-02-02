using Application.Abstractions.Messaging;

namespace Application.Webhooks.Update;

public sealed record UpdateSubscriptionCommand(Guid Id, string? Url, string? EventType, bool? IsActive) : ICommand<UpdateSubscriptionResponseDto>;