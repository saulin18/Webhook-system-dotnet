using Application.Abstractions.Messaging;
using Application.Webhooks.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Domain.Users;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Subscriptions;

internal sealed class DeleteSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("webhooks/subscriptions/{id}", async (
            Guid id,
            ICommandHandler<DeleteSubscriptionCommand, DeleteSubscriptionResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteSubscriptionCommand(id);
            Result<DeleteSubscriptionResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Webhooks)
        .WithName("subscriptions.delete")
        .WithMetadata(new EndpointRequirementMetadata(Permission.WriteTheirOwnWebhooks))
        .RequireAuthorization(Permission.WriteTheirOwnWebhooks);
    }
}