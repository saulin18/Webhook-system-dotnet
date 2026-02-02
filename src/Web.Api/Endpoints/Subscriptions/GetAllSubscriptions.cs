using Application.Webhooks.GetAll;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;
using SharedKernel;
using Web.Api.Endpoints;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Domain.Users;

namespace Web.Api.Endpoints.Webhooks;

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
        .WithName(Permission.WebhooksRead)
        .WithTags(Tags.Webhooks)
        .RequireAuthorization(Permission.ReadTheirOwnWebhooks);
    }
}