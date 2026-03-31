using Application.Webhooks;
using Application.Webhooks.Update;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Domain.Users;
using Infrastructure.Authorization;
namespace Web.Api.Endpoints.Subscriptions;


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
        .WithName("subscriptions.update")
        .WithMetadata(new EndpointRequirementMetadata(Permission.WriteTheirOwnWebhooks))
        .WithTags(Tags.Webhooks)
        .RequireAuthorization(Permission.WriteTheirOwnWebhooks);
    }
}

