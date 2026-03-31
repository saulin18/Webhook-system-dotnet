using Application.Abstractions.Messaging;
using Application.Webhooks.GetById;
using Application.WebhookSubscriptions.GetById;
using Domain.Users;
using Infrastructure.Authorization;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Subscriptions;

internal sealed class GetSubscriptionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("webhooks/subscriptions/{id}", async (
            Guid id,
            IQueryHandler<GetByIdQuery, GetByIdResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetByIdQuery(id);
            Result<GetByIdResponseDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Webhooks)
        .WithName("subscriptions.get-by-id")
        .WithMetadata(new EndpointRequirementMetadata(Permission.ReadTheirOwnWebhooks))
        .RequireAuthorization(Permission.ReadTheirOwnWebhooks);
    }
}