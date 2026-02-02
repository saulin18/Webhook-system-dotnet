using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Infrastructure.Database;
using Web.Api;
using Xunit;

namespace WebhookTests.Integration;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public PostgreSqlContainer? PostgresContainer { get; private set; }

    public async Task InitializeAsync()
    {
        PostgresContainer = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("test")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5433, true)
            .Build();

        await PostgresContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
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
        Environment.SetEnvironmentVariable("RabbitMQ__Host", "localhost");
        Environment.SetEnvironmentVariable("RabbitMQ__Username", "guest");
        Environment.SetEnvironmentVariable("RabbitMQ__Password", "guest");
        Environment.SetEnvironmentVariable("Webhooks__MaxRetries", "3");
        Environment.SetEnvironmentVariable("Webhooks__TimeoutSeconds", "30");

        builder.ConfigureAppConfiguration((context, config) =>
        {
          
            config.Sources.Clear();

            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
           
            ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

         
            ServiceDescriptor? appDbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Application.Abstractions.Data.IApplicationDbContext));

            if (appDbContextDescriptor is not null)
            {
                services.Remove(appDbContextDescriptor);
            }

          
            string connectionString = PostgresContainer!.GetConnectionString();

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public"))
                    .UseSnakeCaseNamingConvention());

            services.AddScoped<Application.Abstractions.Data.IApplicationDbContext>(sp => 
                sp.GetRequiredService<ApplicationDbContext>());
        });

        builder.UseEnvironment("Testing");
    }
}