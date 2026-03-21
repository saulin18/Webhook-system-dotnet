using Application.Abstractions.Messaging;

namespace Application.Webhooks.Update;


public sealed record UpdateSubscriptionCommand(Guid Id, Uri? Url,

string? EventType, bool? IsActive) : ICommand<UpdateSubscriptionResponseDto>;