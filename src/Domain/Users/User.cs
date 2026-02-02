using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public List<string> GetPermissions()
    {
        if (!DefaultPermissions.Permissions.TryGetValue(Role, out List<string>? rolePermissions))
        {
            return [$"owner:{Id}"];
        }


        string ownerValue = $"owner:{Id}";
        List<string> permissions = [.. rolePermissions.Select
        (p => p.Replace("{owner}", ownerValue, StringComparison.OrdinalIgnoreCase))];
        permissions.Add(ownerValue);
        return permissions;
    }
}
