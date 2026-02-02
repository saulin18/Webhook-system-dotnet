using Application.Abstractions.Messaging;
using Application.Abstractions.Authentication;
using Application.Webhooks.CreateSubscription;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using System;

namespace Web.Api.Endpoints.Webhooks;

internal sealed class CreateSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("webhooks/subscriptions", async (
            IUserContext userContext,
            CreateSubscriptionRequest request,
            ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            Console.WriteLine($"User ID: {userContext.UserId}");
            var command = new CreateSubscriptionCommand(request.Url, request.EventType);
            Result<CreateSubscriptionResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Webhooks)
        .RequireAuthorization();
    }

    private sealed record CreateSubscriptionRequest(Uri Url, string EventType);
}