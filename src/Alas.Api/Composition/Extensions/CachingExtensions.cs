using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Alas.Api.Composition.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddBankingCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDistributedCache(configuration);
        services.AddOutputCaching();
        return services;
    }

    private static void AddDistributedCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "ALAS_";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }
    }

    private static void AddOutputCaching(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddPolicy("BranchCache", builder => builder.Expire(TimeSpan.FromMinutes(5)));
            options.AddPolicy("LoanProductCache", builder => builder.Expire(TimeSpan.FromMinutes(10)));
            options.AddPolicy("DashboardCache", builder => builder.Expire(TimeSpan.FromSeconds(30)));
        });
    }
}