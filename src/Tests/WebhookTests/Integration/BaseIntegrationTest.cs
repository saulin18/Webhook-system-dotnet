using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WebhookTests.Integration;
using Domain.Users;
using Xunit;
using Application.Users.Register;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Authentication;
using System.Net.Http.Headers;



namespace WebhookTests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
        Scope = factory.Server.Services.CreateScope();

        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        DbContext.Database.EnsureDeleted();
        DbContext.Database.Migrate();
    }

    public async Task<Tuple<string, User>> GetUserToken(UserRole Role)
    {

        using IServiceScope scope = Scope.ServiceProvider.CreateScope();
        IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var seededUser = SeedingUtils.SeedUser(DbContext, passwordHasher, "test@example.com",
        "Test User", "Test", "User12345", Role);

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