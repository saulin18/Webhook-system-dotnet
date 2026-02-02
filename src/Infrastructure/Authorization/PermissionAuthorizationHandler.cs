using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Infrastructure.Authorization;


internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        var permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

        Guid userId = context.User.GetUserId();
        HashSet<string> userPermissions = await permissionProvider.GetForUserIdAsync(userId);

        if (context.Resource is not HttpContext httpContext)
        {
            return;
        }

        Endpoint? endpoint = httpContext.GetEndpoint();
        string? endpointName = endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;


        string requiredPermission = PermissionsManager.BuildPermissionString(endpointName, requirement.Permission, userId);

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