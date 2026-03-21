using Application.Webhooks;
using Application.Webhooks.Update;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Web.Api.Endpoints;
using Domain.Users;
namespace Web.Api.Endpoints.Webhooks;


internal sealed class UpdateSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("webhooks/subscriptions/{id}", async (
            [AsParameters] UpdateSubscriptionCommand request,
            ICommandHandler<UpdateSubscriptionCommand, UpdateSubscriptionResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSubscriptionCommand(request.Id, request.Url, request.EventType, request.IsActive);
            Result<UpdateSubscriptionResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName(Permission.WebhooksUpdate)
        .WithTags(Tags.Webhooks)
        .RequireAuthorization(Permission.WriteTheirOwnWebhooks);
    }
}

