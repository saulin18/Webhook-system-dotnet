using Application.Abstractions.Messaging;
using Application.Webhooks.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Domain.Users;

namespace Web.Api.Endpoints.Webhooks;

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
        .WithName(Permission.WebhooksDelete)
        .RequireAuthorization(Permission.WriteTheirOwnWebhooks);
    }
}