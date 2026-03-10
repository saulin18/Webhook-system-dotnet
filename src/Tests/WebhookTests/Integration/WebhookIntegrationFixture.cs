using Xunit;

namespace WebhookTests.Integration;

[CollectionDefinition("WebhookIntegration")]
public class WebhookIntegrationCollection : ICollectionFixture<WebhookIntegrationFixture>
{
}

/// <summary>
/// Shared fixture for all webhook integration tests.
/// One factory (and one set of containers: Postgres, RabbitMQ) for the whole collection — not one per test.
/// Containers start once; every test reuses the same app host and DB server, so runs stay fast.
/// </summary>
public sealed class WebhookIntegrationFixture : IAsyncLifetime
{
    public CustomWebApplicationFactory Factory { get; } = new();

    public Task InitializeAsync() => Factory.InitializeAsync();

    public Task DisposeAsync() => Factory.DisposeAsync();
}
