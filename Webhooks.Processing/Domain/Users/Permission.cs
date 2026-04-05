namespace Webhooks.Processing.Domain.Users;

public static class Permission
{
    public const string UsersRead = "users.read";
    public const string UsersWrite = "users.write";
    public const string UsersDelete = "users.delete";
    public const string UsersAll = "users.all";
    public const string WebhooksRead = "webhooks.read";
    public const string WebhooksWrite = "webhooks.write";
    public const string WebhooksUpdate = "webhooks.update";
    public const string WebhooksDelete = "webhooks.delete";
    public const string WebhooksAll = "webhooks.all";
    public const string AdminAccess = "admin.access";
    public const string SystemManage = "system.manage";
    public const string ReadTheirOwnWebhooks = "webhooks.read.owner:{owner}";
    public const string WriteTheirOwnWebhooks = "webhooks.write.owner:{owner}";
    public const string ReadTheirOwnProfile = "users.read.owner:{owner}";
    public const string WriteTheirOwnProfile = "users.write.owner:{owner}";
}
