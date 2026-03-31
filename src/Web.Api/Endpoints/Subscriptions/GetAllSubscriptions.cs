using Application.Webhooks.GetAll;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Domain.Users;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Subscriptions;

internal sealed class GetAllSubscriptions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("webhooks/subscriptions", async (
            [AsParameters] GetAllSubscriptionsQuery query,
            IQueryHandler<GetAllSubscriptionsQuery, GetAllSubscriptionsResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            Result<GetAllSubscriptionsResponseDto> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("subscriptions.get-all")
        .WithMetadata(new EndpointRequirementMetadata(Permission.ReadTheirOwnWebhooks))
        .WithTags(Tags.Webhooks)
        .RequireAuthorization(Permission.ReadTheirOwnWebhooks);
    }
}