using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal == null)
        {
            throw new ApplicationException("ClaimsPrincipal is null");
        }

        string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub) 
            ?? principal.FindFirstValue("sub")
            ?? principal.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            
            var allClaims = principal.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
            throw new ApplicationException($"User id claim not found. Available claims: {string.Join(", ", allClaims)}");
        }

        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            throw new ApplicationException($"User id '{userId}' is not a valid GUID");
        }

        return parsedUserId;
    }
}
