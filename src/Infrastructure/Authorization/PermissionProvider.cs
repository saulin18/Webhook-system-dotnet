using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class PermissionProvider (ApplicationDbContext context)
{

    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        User user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return [];
        }
        return user.GetPermissions().ToHashSet();
   
    }
}