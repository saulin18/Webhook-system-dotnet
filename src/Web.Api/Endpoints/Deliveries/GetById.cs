using Application.Abstractions.Messaging;
using Application.WebHookDeliveries.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Web.Api.Endpoints;
using Domain.Users;

namespace Web.Api.Endpoints.Deliveries;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("webhooks/deliveries/{id}", async (
            Guid id,
            IQueryHandler<GetByIdQuery, GetByIdResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetByIdQuery(id);
            Result<GetByIdResponseDto> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Webhooks)
        .RequireAuthorization(Permission.WebhooksRead);
    }
}