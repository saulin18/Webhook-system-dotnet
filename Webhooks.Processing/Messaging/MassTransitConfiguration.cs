using MassTransit;


namespace Webhooks.Processing.Messaging;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitProcessing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<WebHookDispatcherConsumer>();
            x.AddConsumer<WebHookTriggeredConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMQ");
                string host = rabbitMqConfig["Host"] ?? "localhost";
                ushort? port = rabbitMqConfig.GetValue<ushort?>("Port") ?? 5672;
                cfg.Host(host, port.Value, "/", h =>
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
