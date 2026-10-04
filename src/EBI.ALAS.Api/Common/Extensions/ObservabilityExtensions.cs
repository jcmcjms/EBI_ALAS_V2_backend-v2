using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace EBI.ALAS.Api.Common.Extensions;

public static class ObservabilityExtensions
{
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "EBI.ALAS.Api")
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static IServiceCollection AddBankingObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("EBI.ALAS.Api"))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddConsoleExporter();
            })
            .WithLogging(logging =>
            {
                logging.AddConsoleExporter();
            });

        return services;
    }

    public static IServiceCollection AddBankingHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();

        var sqlConnection = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(sqlConnection))
        {
            builder.AddSqlServer(sqlConnection, name: "sqlserver", tags: ["db", "sql", "ready"]);
        }

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            builder.AddRedis(redisConnection, name: "redis", tags: ["cache", "redis", "ready"]);
        }

        builder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        return services;
    }
}