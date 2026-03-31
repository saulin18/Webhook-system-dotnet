
using System.Net.Http.Json;
using Application.WebHookDeliveries;
using Domain.Users;
using Xunit;

namespace WebhookTests.Integration.Deliveries;

public sealed class GetAllDeliveriesTest(WebhookIntegrationFixture fixture) : BaseIntegrationTest(fixture)
{
    private readonly string _endpoint = "/webhooks/deliveries?pageSize=10";

    [Fact]
    public async Task GetAllDeliveries_WithValidData_ReturnsSuccess()
    {

        var (token, user) = await GetUserToken(Role: UserRole.User);
        await SeedingUtils.SeedWebhookDeliveries(DbContext, user);
        SetupHttpClientWithToken(token);
        HttpResponseMessage response = await Client.GetAsync(_endpoint);
        response.EnsureSuccessStatusCode();
        GetAllDeliveriesResponseDto? deliveries = await response.Content.ReadFromJsonAsync<GetAllDeliveriesResponseDto>();
        Assert.NotNull(deliveries);
        Assert.NotEmpty(deliveries.PagedResult.Items);
    }
}
