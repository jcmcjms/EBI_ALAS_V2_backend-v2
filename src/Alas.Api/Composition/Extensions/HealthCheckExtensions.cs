using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Alas.Api.Composition.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddBankingHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis");
        var rabbitMqConnection = configuration.GetConnectionString("RabbitMQ");

        var healthChecksBuilder = services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "sqlserver",
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(5));

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            healthChecksBuilder.AddCheck<GarnetHealthCheck>(
                "garnet",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(5));
        }

        if (!string.IsNullOrWhiteSpace(rabbitMqConnection))
        {
            healthChecksBuilder.AddRabbitMQ(
                sp =>
                {
                    var factory = new ConnectionFactory
                    {
                        Uri = new Uri(rabbitMqConnection!),
                        AutomaticRecoveryEnabled = true
                    };
                    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
                },
                name: "rabbitmq",
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(5));
        }

        return services;
    }
}