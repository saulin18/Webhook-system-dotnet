using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using WebhookTests.Integration;
using Web.Api.Endpoints.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstractions.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;
using Application.Webhooks.CreateSubscription;
using System.Net.Http;
using System.Net.Http.Json;
using SharedKernel;
using Microsoft.AspNetCore.Http;
using Web.Api.Infrastructure;
using Domain.Users;


namespace WebhookTests.Integration.Subscriptions;

public sealed class CreateSubscriptionTest : BaseIntegrationTest
{

    private readonly string _endpoint = "webhooks/subscriptions";

    public CreateSubscriptionTest(WebhookIntegrationFixture fixture) : base(fixture)
    {

    }

    [Fact]
    public async Task CreateSubscription_WithValidData_ReturnsSuccess()
    {
        var requestBody = new
        {
            Url = "https://example.com/webhook",
            EventType = "user.created"
        };
        var (token, seededUser) = await GetUserToken(UserRole.User);


        SetupHttpClientWithToken(token);

        HttpResponseMessage response = await Client.PostAsJsonAsync(_endpoint, requestBody);

        response.EnsureSuccessStatusCode();
        Assert.True(response.IsSuccessStatusCode);
        var responseBody = await response.Content.ReadFromJsonAsync<CreateSubscriptionResponseDto>();
        Assert.NotNull(responseBody);
        Assert.Equal(seededUser.Id, responseBody.UserId);

    }

    [Fact]
    public async Task CreateSubscription_WithInvalidData_ReturnsError()
    {
        var requestBody = new
        {
            Url = "invalid-url",
            EventType = "user.created"
        };

        var (token, _) = await GetUserToken(UserRole.User);

        SetupHttpClientWithToken(token);

        HttpResponseMessage response = await Client.PostAsJsonAsync(_endpoint, requestBody);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(400, problemDetails.Status);

        // Verificar que el detalle contiene información sobre el error de validación
        Assert.NotNull(problemDetails.Detail);
        Assert.Contains("validation", problemDetails.Detail, StringComparison.OrdinalIgnoreCase);

    }
}
