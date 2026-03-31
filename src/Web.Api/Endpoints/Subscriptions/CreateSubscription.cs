using Application.Abstractions.Messaging;
using Application.Abstractions.Authentication;
using Application.Webhooks.CreateSubscription;
using Application.WebhookSubscriptions.Create;
using Domain.Users;
using Infrastructure.Authorization;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;


namespace Web.Api.Endpoints.Subscriptions;

internal sealed class CreateSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("webhooks/subscriptions", async (
            IUserContext userContext,
            CreateSubscriptionCommand request,
            ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            Console.WriteLine($"User ID: {userContext.UserId}");
            var command = new CreateSubscriptionCommand(request.Url, request.EventType);
            Result<CreateSubscriptionResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Webhooks)
        .WithName("subscriptions.create")
        .WithMetadata(new EndpointRequirementMetadata(Permission.WriteTheirOwnWebhooks))
        .RequireAuthorization(Permission.WriteTheirOwnWebhooks);
    }

 
}