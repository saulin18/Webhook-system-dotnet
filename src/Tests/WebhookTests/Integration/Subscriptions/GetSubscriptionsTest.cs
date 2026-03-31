using Xunit;
using Application.Webhooks.GetAll;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Application.Webhooks.GetById;
using Domain.Users;
using Domain.Webhooks;

namespace WebhookTests.Integration.Subscriptions;

public sealed class GetSubscriptionsTest(WebhookIntegrationFixture fixture) : BaseIntegrationTest(fixture)
{
    private readonly string _endpoint = "webhooks/subscriptions";
    private readonly string _detailEndpoint = "webhooks/subscriptions/{id}";

    [Fact]
    public async Task GetSubscriptions_WithValidData_ReturnsSuccess()
    {
        var (token, _) = await GetUserToken(Role: UserRole.User);
        SetupHttpClientWithToken(token);
        HttpResponseMessage response = await Client.GetAsync(_endpoint + "?page=1&pageSize=10");
        //response.EnsureSuccessStatusCode();
        Assert.True(response.IsSuccessStatusCode);
        GetAllSubscriptionsResponseDto? responseBody = await response.Content.ReadFromJsonAsync<GetAllSubscriptionsResponseDto>();
        Assert.NotNull(responseBody);
    }


    [Fact]
    public async Task GetSubscriptions_WithoutAuth_ReturnsError()
    {

        HttpResponseMessage response = await Client.GetAsync(_endpoint + "?page=1&pageSize=10");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSubscription_ById_WithValidData_ReturnsSuccess()
    {
        var (token, user) = await GetUserToken(Role: UserRole.User);
        SetupHttpClientWithToken(token);
        WebhookSubscription subscription = await SeedingUtils.SeedSubscription(DbContext, SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0]);

        HttpResponseMessage response = await Client.GetAsync(_detailEndpoint.Replace("{id}", subscription.Id.ToString()));
        response.EnsureSuccessStatusCode();
        GetByIdResponseDto? responseBody = await response.Content.ReadFromJsonAsync<GetByIdResponseDto>();
        Assert.NotNull(responseBody);
       Assert.Equal(subscription.Id, responseBody.Id);
    }

    [Fact]
    public async Task GetSubscription_ById_WithoutAValidId_ReturnsErrors()
    {
        var (token, user) = await GetUserToken(Role: UserRole.User);
        SetupHttpClientWithToken(token);
        //Subscription isnt in database
        WebhookSubscription subscription = SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0];
        HttpResponseMessage response = await Client.GetAsync(_detailEndpoint.Replace("{id}", subscription.Id.ToString()));
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

}