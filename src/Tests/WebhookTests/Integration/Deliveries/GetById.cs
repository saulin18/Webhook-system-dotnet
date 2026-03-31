using System.Net.Http.Json;
using Application.WebHookDeliveries.GetById;
using Domain.Users;
using Domain.Webhooks;
using Xunit;

namespace WebhookTests.Integration.Deliveries;

public sealed class GetDeliveryByIdTest(WebhookIntegrationFixture fixture) : BaseIntegrationTest(fixture)
{
    private readonly string _endpoint = "/webhooks/deliveries/{id}";

    [Fact]
    public async Task GetDeliveryById_WithValidData_ReturnsSuccess()
    {

        (string token, User user) = await GetUserToken(Role: UserRole.User);
        (_, List<WebhookDelivery> seededDeliveries) = await SeedingUtils.SeedWebhookDeliveries(DbContext, user);
        SetupHttpClientWithToken(token);
        HttpResponseMessage response = await Client.GetAsync(_endpoint.Replace("{id}", seededDeliveries[0].Id.ToString()));
        response.EnsureSuccessStatusCode();
        GetByIdResponseDto? delivery = await response.Content.ReadFromJsonAsync<GetByIdResponseDto>();
        Assert.NotNull(delivery);
        Assert.Equal(seededDeliveries[0].Id, delivery.Id);
    }
}
