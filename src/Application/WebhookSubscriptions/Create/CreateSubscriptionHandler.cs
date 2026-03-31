using System.Security.Cryptography;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Webhooks.CreateSubscription;
using Domain.Webhooks;
using SharedKernel;

namespace Application.WebhookSubscriptions.Create;

internal sealed class CreateSubscriptionHandler(
    IApplicationDbContext context, 
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponseDto>
{

    public async Task<Result<CreateSubscriptionResponseDto>> Handle(CreateSubscriptionCommand command,
    CancellationToken cancellationToken)
    {
        var subscription = new WebhookSubscription
        {
            UserId = userContext.UserId,
            Url = command.Url.ToString(),
            Secret = GenerateSecret(),
            EventType = command.EventType,
            IsActive = true,
            CreatedAt = dateTimeProvider.UtcNow
        };

        context.WebhookSubscriptions.Add(subscription);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateSubscriptionResponseDto(subscription.Id, subscription.CreatedAt, subscription.UserId);
    }

    private static string GenerateSecret()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}

