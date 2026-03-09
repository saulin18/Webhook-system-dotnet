using Infrastructure.WebHookDispatcher;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<WebHookDispatcherConsumer>();
            x.AddConsumer<WebHookTriggeredConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMQ");
                var host = rabbitMqConfig["Host"] ?? "localhost";
                var port = rabbitMqConfig.GetValue<ushort?>("Port") ?? 5672;
                cfg.Host(host, port, "/", h =>
                {
                    h.Username(rabbitMqConfig["Username"] ?? "guest");
                    h.Password(rabbitMqConfig["Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
