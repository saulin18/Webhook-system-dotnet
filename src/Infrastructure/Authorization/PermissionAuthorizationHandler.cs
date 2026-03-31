
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        PermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

        Guid userId = context.User.GetUserId();
        HashSet<string> userPermissions = await permissionProvider.GetForUserIdAsync(userId);

        if (context.Resource is not HttpContext httpContext)
        {
            return;
        }

        Endpoint? endpoint = httpContext.GetEndpoint();
        //We can't use the endpoint name since this needs to be unique but the permissions
        //is something that can be shared between endpoints.
        //string? endpointRequirement = endpoint?.Metadata.GetMetadata<endpointRequirementMetadata>()?.endpointRequirement;
        string? endpointRequirement = endpoint?.Metadata.GetRequiredMetadata<EndpointRequirementMetadata>().EndpointRequirement; 
        string requiredPermission = PermissionsManager.BuildPermissionString(
            endpointRequirement ?? string.Empty,
            requirement.Permission,
            userId
        );

        if (string.IsNullOrEmpty(requiredPermission))
        {
            return;
        }

        if (PermissionsManager.HasPermission(requiredPermission, userPermissions))
        {
            context.Succeed(requirement);
        }
    }
}
