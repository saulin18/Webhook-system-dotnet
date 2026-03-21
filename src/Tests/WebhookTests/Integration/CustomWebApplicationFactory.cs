using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Web.Api;
using Xunit;

namespace WebhookTests.Integration;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const int TestServerPort = 5099;

    public static readonly string TestWebhookReceiverUrl = $"http://localhost:{TestServerPort}/test/webhook-receiver";

    private const string RabbitUser = "test";
    private const string RabbitPass = "test";

    private static CustomWebApplicationFactory? s_instance;

    public PostgreSqlContainer? PostgresContainer { get; private set; }
    public RabbitMqContainer? RabbitMqContainer { get; private set; }

    public async Task InitializeAsync()
    {
        PostgresContainer = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("test")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5433, true)
            .Build();

        RabbitMqContainer = new RabbitMqBuilder("rabbitmq:3")
            .WithImage("rabbitmq:3-management")
            .WithUsername(RabbitUser)
            .WithPassword(RabbitPass)
            .WithPortBinding(5672, true)
            .Build();

        await PostgresContainer.StartAsync();
        await RabbitMqContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        if (RabbitMqContainer is not null)
        {
            await RabbitMqContainer.DisposeAsync();
        }


        if (PostgresContainer is not null)
        {
            await PostgresContainer.DisposeAsync();
        }


        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        string connectionString = PostgresContainer!.GetConnectionString();
        Environment.SetEnvironmentVariable("ConnectionStrings__Database", connectionString);
        Environment.SetEnvironmentVariable("Jwt__Secret", "super-duper-secret-value-that-should-be-in-user-secrets");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "clean-architecture");
        Environment.SetEnvironmentVariable("Jwt__Audience", "developers");
        Environment.SetEnvironmentVariable("Jwt__ExpirationInMinutes", "60");
        Environment.SetEnvironmentVariable("RabbitMQ__Host", RabbitMqContainer!.Hostname);
        Environment.SetEnvironmentVariable("RabbitMQ__Port", RabbitMqContainer.GetMappedPublicPort(5672).ToString(System.Globalization.CultureInfo.InvariantCulture));
        Environment.SetEnvironmentVariable("RabbitMQ__Username", RabbitUser);
        Environment.SetEnvironmentVariable("RabbitMQ__Password", RabbitPass);
        Environment.SetEnvironmentVariable("Webhooks__MaxRetries", "3");
        Environment.SetEnvironmentVariable("Webhooks__TimeoutSeconds", "30");
        Environment.SetEnvironmentVariable("Testing__Enabled", "true");
        Environment.SetEnvironmentVariable("Testing__ServerPort", TestServerPort.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Environment.SetEnvironmentVariable("Testing__WebhookReceiverUrl", TestWebhookReceiverUrl);

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddEnvironmentVariables();
        });

        builder.ConfigureTestServices(services => services.AddHttpClient("Webhooks").ConfigurePrimaryHttpMessageHandler(GetTestServerHandler));

        builder.UseUrls($"http://localhost:{TestServerPort}");
        builder.UseEnvironment("Testing");
    }


    internal void RegisterForTestServerHandler() => s_instance = this;

    private static HttpMessageHandler GetTestServerHandler() => s_instance!.Server.CreateHandler();
}