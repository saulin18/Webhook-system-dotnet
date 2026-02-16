using System.Net.Http.Json;
using Application.Webhooks.Delete;
using Domain.Users;
using Domain.Webhooks;
using SharedKernel;
using WebhookTests;
using Xunit;

namespace WebhookTests.Integration;

public sealed class DeleteSubscriptionTest : BaseIntegrationTest
{
    private readonly string endpoint = "webhooks/subscriptions/{id}";

    public DeleteSubscriptionTest(CustomWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task DeleteSubscription_WithValidData_ReturnsSuccess()
    {
        var (token, user) = await GetUserToken(UserRole.User);
        SetupHttpClientWithToken(token);
        var subscription = await SeedingUtils.SeedSubscription(
            DbContext,
            SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0]
        );

        var response = await Client.DeleteAsync(
            endpoint.Replace("{id}", subscription.Id.ToString())
        );
        response.EnsureSuccessStatusCode();
        Assert.True(response.IsSuccessStatusCode);
        var responseBody =
            await response.Content.ReadFromJsonAsync<DeleteSubscriptionResponseDto>();
        Assert.NotNull(responseBody);
        Assert.Equal(subscription.Id, responseBody.Id);
    }

    [Fact]
    public async Task DeleteSubscription_WithInvalidId_Returns404()
    {
        var (token, user) = await GetUserToken(UserRole.User);
        SetupHttpClientWithToken(token);
        var subscription = SeedingUtils.GetSeedingWebhookSubscriptions(user.Id)[0];

        var response = await Client.DeleteAsync(
            endpoint.Replace("{id}", subscription.Id.ToString())
        );
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
