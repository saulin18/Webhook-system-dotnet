using Infrastructure.WebHookDispatcher;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x => x.UsingRabbitMq((context, cfg) =>
        {
            IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMQ");
            x.AddConsumer<WebHookDispatcherConsumer>();
            x.AddConsumer<WebHookTriggeredConsumer>();
            
            cfg.Host(rabbitMqConfig["Host"]!, h =>
            {
                h.Username(rabbitMqConfig["Username"]!);
                h.Password(rabbitMqConfig["Password"]!);
            });

            cfg.ConfigureEndpoints(context);
        }));

        return services;
    }
}
