using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Webhooks.Processing.DependencyInjection;
using Webhooks.Processing.Extensions;
using Webhooks.Processing.Messaging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddHealthChecks(builder.Configuration)
    .AddAuthenticationServices(builder.Configuration)
    .AddWebhookServices(builder.Configuration)
    .AddMassTransitProcessing(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

await app.RunAsync();

namespace Webhooks.Processing
{
    public partial class Program;
}

