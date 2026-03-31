using System.Net.Http.Headers;
using Application.Abstractions.Authentication;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WebhookTests.Integration;

/// <summary>
/// Base for integration tests. Uses the shared factory from the fixture (same containers and app for all tests).
/// Each test gets its own Client, Scope, and DbContext, and starts with an empty DB (EnsureDeleted + Migrate).
/// </summary>
[Collection("WebhookIntegration")]
public abstract class BaseIntegrationTest : IDisposable
{
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(WebhookIntegrationFixture fixture)
    {
        CustomWebApplicationFactory factory = fixture.Factory;
        Client = factory.CreateClient();
        factory.RegisterForTestServerHandler();
        Scope = factory.Server.Services.CreateScope();

        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        DbContext.Database.EnsureDeleted();
        DbContext.Database.Migrate();
    }

    public async Task<Tuple<string, User>> GetUserToken(UserRole Role)
    {
        using IServiceScope scope = Scope.ServiceProvider.CreateScope();
        IPasswordHasher passwordHasher =
            scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        User seededUser = SeedingUtils.SeedUser(
            DbContext,
            passwordHasher,
            "test@example.com",
            "Test User",
            "Test",
            "User12345",
            Role
        );

        string token = await SeedingUtils.LoginUser(Client, "test@example.com", "User12345");

        return Tuple.Create(token, seededUser);
    }

    public void SetupHttpClientWithToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = null;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        Scope.Dispose();
        DbContext.Dispose();
    }
}
