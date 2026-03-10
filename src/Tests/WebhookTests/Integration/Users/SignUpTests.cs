using WebhookTests.Integration;
using Domain.Users;
using Application.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Application.Users.Register;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;


namespace WebhookTests.Integration.Users;

public sealed class SignUpEndpointTests : BaseIntegrationTest
{
    private readonly string _endpoint = "users/register";
    public SignUpEndpointTests(WebhookIntegrationFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SignUpUser_WithValidData_ReturnSuccess()
    {

        var requestBody = new
        {
            Email = "test@gmail.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!"
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync(_endpoint, requestBody);

        response.EnsureSuccessStatusCode();

        User? userInDb = await DbContext.Users.SingleOrDefaultAsync(user => user.Email == requestBody.Email);


        Assert.NotNull(userInDb);
        Assert.Equal(requestBody.Email, userInDb?.Email);
        Assert.Equal(requestBody.FirstName, userInDb?.FirstName);
        Assert.Equal(requestBody.LastName, userInDb?.LastName);
    }

    [Fact]
    public async Task SignUpUser_WithExistingEmail_ReturnsBadRequest()
    {
        var requestBody = new
        {
            Email = "test@gmail.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!"
        };
        // First registration should succeed
        HttpResponseMessage firstResponse = await Client.PostAsJsonAsync(_endpoint, requestBody);
        firstResponse.EnsureSuccessStatusCode();

        // Second registration with the same email should fail
        HttpResponseMessage secondResponse = await Client.PostAsJsonAsync(_endpoint, requestBody);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task SignUpUser_WithInvalidEmail_ReturnsBadRequest()
    {
        var requestBody = new
        {
            Email = "invalid-email",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!"
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync(_endpoint, requestBody);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

}