using Application.Abstractions.Messaging;
using Application.WebHookDeliveries;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Web.Api.Endpoints;
using Domain.Users;
namespace Web.Api.Endpoints.Deliveries;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("webhooks/deliveries", async (
            [AsParameters] GetAllDeliveriesQuery query,
            IQueryHandler<GetAllDeliveriesQuery, GetAllDeliveriesResponseDto> handler,
            CancellationToken cancellationToken) => 
        {
            Result<GetAllDeliveriesResponseDto> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Webhooks)
        .RequireAuthorization(Permission.WebhooksRead);
    }
}