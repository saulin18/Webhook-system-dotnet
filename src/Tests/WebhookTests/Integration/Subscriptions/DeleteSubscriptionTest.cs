using System.Net.Http.Json;
using Application.Webhooks.Delete;
using Domain.Users;
using Domain.Webhooks;
using Xunit;

namespace WebhookTests.Integration.Subscriptions;

public sealed class DeleteSubscriptionTest(WebhookIntegrationFixture fixture) : BaseIntegrationTest(fixture)
{
    private readonly string _endpoint = "webhooks/subscriptions/{id}";

    [Fact]
    public async Task DeleteSubscription_WithValidData_ReturnsSuccess()
    {
        var (token, user) = await GetUserToken(Role: UserRole.User);
        SetupHttpClientWithToken(token);
        WebhookSubscription subscription = await SeedingUtils.SeedSubscription(
            DbContext,
            SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0]
        );

        HttpResponseMessage response = await Client.DeleteAsync(
            _endpoint.Replace("{id}", subscription.Id.ToString())
        );
        response.EnsureSuccessStatusCode();
        Assert.True(response.IsSuccessStatusCode);
        DeleteSubscriptionResponseDto? responseBody =
            await response.Content.ReadFromJsonAsync<DeleteSubscriptionResponseDto>();
        Assert.NotNull(responseBody);
        Assert.Equal(subscription.Id, responseBody.Id);
    }

    [Fact]
    public async Task DeleteSubscription_WithInvalidId_Returns404()
    {
        var (token, user) = await GetUserToken(Role: UserRole.User);
        SetupHttpClientWithToken(token);
        WebhookSubscription subscription = SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0];

        HttpResponseMessage response = await Client.DeleteAsync(
            _endpoint.Replace("{id}", subscription.Id.ToString())
        );
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
