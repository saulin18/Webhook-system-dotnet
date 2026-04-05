using Webhooks.Processing.Domain.Users;
using Webhooks.Processing.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;

namespace Webhooks.Processing.Database;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
  
    DbSet<WebhookSubscription> WebhookSubscriptions { get; }

    DbSet<WebhookDelivery> WebhookDeliveries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
