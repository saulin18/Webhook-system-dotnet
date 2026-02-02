using Xunit;
using Domain.Webhooks;
using WebhookTests;
using SharedKernel;
using Application.Webhooks.Update;
using Domain.Users;
using System.Net.Http.Json;
using Application.Webhooks;

namespace WebhookTests.Integration.Subscriptions;

public sealed class UpdateSubscriptionTest : BaseIntegrationTest
{
    private readonly string _endpoint = "webhooks/subscriptions/{id}";

    public UpdateSubscriptionTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateSubscription_WithValidData_ReturnsSuccess()
    {
        var (token, user) = await GetUserToken(UserRole.User);
        SetupHttpClientWithToken(token);

        WebhookSubscription subscription = await SeedingUtils.SeedSubscription
        (DbContext, SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0]);

        var requestBody = new
        {
            Url = subscription.Url + "test",
            EventType = subscription.EventType +" test"
        };

        var response = await Client.PatchAsJsonAsync(_endpoint.Replace("{id}", subscription.Id.ToString()), requestBody);
        //response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<UpdateSubscriptionResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(body.Id, subscription.Id);
    }


    [Fact]
    public async Task UpdateSubscription_WithInvalidData_ReturnsBadRequest()
    {
        var (token, user) = await GetUserToken(UserRole.User);
        SetupHttpClientWithToken(token);

        WebhookSubscription subscription = await SeedingUtils.SeedSubscription
        (DbContext, SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0]);

        var requestBody = new
        {
            Url = "",
            EventType = ""
        };

        var response = await Client.PatchAsJsonAsync(_endpoint.Replace("{id}", subscription.Id.ToString()), requestBody);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        
    }


}