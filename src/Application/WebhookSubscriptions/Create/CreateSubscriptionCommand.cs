using Application.Abstractions.Messaging;
using Application.Webhooks.CreateSubscription;

namespace Application.WebhookSubscriptions.Create;
 
public sealed record CreateSubscriptionCommand(
    Uri Url,
    string EventType
) : ICommand<CreateSubscriptionResponseDto>;   
