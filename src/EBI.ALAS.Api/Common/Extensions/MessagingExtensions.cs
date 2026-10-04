using MassTransit;
using EBI.ALAS.Api.Infrastructure.Messaging.Consumers;
using EBI.ALAS.Api.Infrastructure.Messaging;

namespace EBI.ALAS.Api.Common.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddBankingMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConnection = configuration.GetConnectionString("RabbitMQ");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<AuditLogConsumer>();
            x.AddConsumer<NotificationConsumer>();

            if (!string.IsNullOrEmpty(rabbitMqConnection))
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConnection);
                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }
}