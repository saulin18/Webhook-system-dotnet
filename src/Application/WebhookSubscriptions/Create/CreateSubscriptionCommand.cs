using Application.Abstractions.Messaging;

namespace Application.Webhooks.CreateSubscription;
 
public sealed record CreateSubscriptionCommand(
    Uri Url,
    string EventType
) : ICommand<CreateSubscriptionResponseDto>;   
