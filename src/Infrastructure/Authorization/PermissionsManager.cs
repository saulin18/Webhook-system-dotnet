using Domain.Users;
using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Authorization;


namespace Infrastructure.Authorization;
public sealed class PermissionsManager
{

    private static readonly Dictionary<string, Regex> Wildcards = new()
    {
        { "all", new Regex("^.+$") },
        { "read", new Regex("^(get|getbyid|list)$", RegexOptions.IgnoreCase) },
        { "write", new Regex("^(create|post|update|put|delete)$", RegexOptions.IgnoreCase) }
    };

    public static bool HasPermission(string requiredPermission, HashSet<string> userPermissions)
    {
        foreach (var userPermission in userPermissions)
        {
            if (Match(requiredPermission, userPermission))
            {
                return true;
            }
        }
        return false;
    }

    public static string BuildPermissionString(string endpointName, string permission, Guid userId)
    {
        if (string.IsNullOrEmpty(endpointName))
        {
            return string.Empty;
        }

        var parts = endpointName.Split('.');
        if (parts.Length < 2)
        {
            return string.Empty;
        }

        string resource = parts[0].ToLower();
        string action = parts[1].ToLower(); 

        string mappedAction = MapActionName(action);

        string requiredPermission = permission
            .Replace("{resource}", resource, StringComparison.OrdinalIgnoreCase)
            .Replace("{action}", mappedAction, StringComparison.OrdinalIgnoreCase)
            .Replace("{owner}", $"owner:{userId}", StringComparison.OrdinalIgnoreCase);

        return requiredPermission;
    }
    private static bool Match(string requiredPermission, string userPermission)
    {
        var requiredParts = requiredPermission.Split('.');
        var userParts = userPermission.Split('.');

        if (requiredParts.Length != userParts.Length)
        {
            return false;
        }

        for (int i = 0; i < requiredParts.Length; i++)
        {
            string requiredPart = requiredParts[i];
            string userPart = userParts[i];

            // Verificar wildcards
            if (Wildcards.TryGetValue(userPart, out var regex) && regex.IsMatch(requiredPart))
            {
                continue;
            }

            // Verificar coincidencia exacta
            if (string.Equals(requiredPart, userPart, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Verificar ownership
            if (requiredPart.StartsWith("owner:") && userPart.StartsWith("owner:"))
            {
                string requiredId = requiredPart.Split(':')[1];
                string userId = userPart.Split(':')[1];
                if (string.Equals(requiredId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }
            return false;
        }
        return true;
    }

    // Mapear nombres de acciones a permisos estándar
    private static string MapActionName(string actionName)
    {
        return actionName.ToLower() switch
        {
            var action when action.StartsWith("get") || action.StartsWith("list") => "read",
            var action when action.StartsWith("create") || action.StartsWith("post") => "write",
            var action when action.StartsWith("update") || action.StartsWith("put") => "write",
            var action when action.StartsWith("delete") => "write",
            _ => actionName.ToLower()
        };
    }
}