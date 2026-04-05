using Microsoft.EntityFrameworkCore;
using Webhooks.Processing.Domain.Users;
using Webhooks.Processing.Domain.Webhooks;

namespace Webhooks.Processing.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }

    public DbSet<WebhookDelivery> WebhookDeliveries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema("webhooks_processing");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}
