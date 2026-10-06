using Alas.Api.Infrastructure.Messaging;
using Alas.Api.Infrastructure.Messaging.Consumers;
using MassTransit;
using StackExchange.Redis;

namespace Alas.Api.Composition.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddBankingMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransitMessaging(configuration);
        services.AddSignalRBackplane(configuration);
        return services;
    }

    private static void AddMassTransitMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqConnection = configuration.GetConnectionString("RabbitMQ");

        if (!string.IsNullOrWhiteSpace(rabbitMqConnection))
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AuditLogConsumer>();
                x.AddConsumer<NotificationConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConnection);
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
        else
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AuditLogConsumer>();
                x.AddConsumer<NotificationConsumer>();
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
        }

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
    }

    private static void AddSignalRBackplane(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis");

        var signalRBuilder = services.AddSignalR();

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            signalRBuilder.AddStackExchangeRedis(redisConnection, options =>
            {
                options.Configuration.ChannelPrefix =
                    RedisChannel.Literal("ALAS_SignalR");
                options.Configuration.ReconnectRetryPolicy = new ExponentialRetry(5000);
                options.Configuration.KeepAlive = 30;
                options.Configuration.AbortOnConnectFail = false;
            });
        }
    }
}