using Xunit;
using Domain.Users;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstractions.Authentication;

namespace WebhookTests.Integration.Users;
public sealed class LoginIntegrationTests : BaseIntegrationTest
{
    private readonly string _endpoint = "users/login";
    public LoginIntegrationTests(WebhookIntegrationFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsSuccess()
    {

        using IServiceScope scope = Scope.ServiceProvider.CreateScope();
        IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();


        User addedUser = SeedingUtils.SeedUser(DbContext, passwordHasher, "test@gmail.com", "User", "Test", "Password123!", UserRole.User);
        Assert.NotNull(addedUser);

        var loginRequest = new
        {
            Email = "test@gmail.com",
            Password = "Password123!"
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync(_endpoint, loginRequest);
        response.EnsureSuccessStatusCode();

        string token = await response.Content.ReadAsStringAsync();


        Assert.NotEmpty(token);
        string[] parts = token.Split('.');
        Assert.Equal(3, parts.Length);

    }

    [Fact]
    public async Task LoginUser_WithInvalidCredentials_ReturnsError()
    {


        using IServiceScope scope = Scope.ServiceProvider.CreateScope();
        IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        SeedingUtils.SeedUser(DbContext, passwordHasher, "test@gmail.com", "Password123!", "test", "user", UserRole.User);

        var request = new
        {
            Email = "test@gmail.com",
            Password = "Password123" //Invalid password
        };

        var response = await Client.PostAsJsonAsync(_endpoint, request);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}