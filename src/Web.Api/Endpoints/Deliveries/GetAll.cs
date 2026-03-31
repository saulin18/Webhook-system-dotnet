using Application.Abstractions.Messaging;
using Application.WebHookDeliveries;
using Application.WebHookDeliveries.GetAll;
using Domain.Users;
using Infrastructure.Authorization;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;


namespace Web.Api.Endpoints.Deliveries;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "webhooks/deliveries",
                async (
                    [AsParameters] GetAllDeliveriesQuery query,
                    IQueryHandler<GetAllDeliveriesQuery, GetAllDeliveriesResponseDto> handler,
                    CancellationToken cancellationToken
                ) =>
                {
                    Result<GetAllDeliveriesResponseDto> result = await handler.Handle(
                        query,
                        cancellationToken
                    );
                    return result.Match(Results.Ok, CustomResults.Problem);
                }
            )
            .WithTags(Tags.Webhooks)
            .WithName("deliveries.get-all")
            .WithMetadata(new EndpointRequirementMetadata(Permission.ReadTheirOwnWebhooks))
            .RequireAuthorization(Permission.ReadTheirOwnWebhooks);
    }
}
