namespace Domain.Users;


public static class DefaultPermissions
{
    internal static readonly Dictionary<UserRole, List<string>> Permissions = new()
    {
        {
            UserRole.Admin,
            [
                Permission.UsersAll,
                Permission.WebhooksAll,
                Permission.AdminAccess,
                Permission.SystemManage,
            ]
        },
        {
            UserRole.User,
            [
                Permission.ReadTheirOwnProfile,
                Permission.WriteTheirOwnProfile,
                Permission.ReadTheirOwnWebhooks,
                Permission.WriteTheirOwnWebhooks,
            ]
        },
    };
}