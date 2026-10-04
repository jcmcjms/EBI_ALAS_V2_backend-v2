using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using EBI.ALAS.Api.Infrastructure.Caching;

namespace EBI.ALAS.Api.Common.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddBankingCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "ALAS_";
            });

            services.AddSingleton<GarnetConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(redisConnection);
                config.AbortOnConnectFail = false;
                return new GarnetConnectionMultiplexer(ConnectionMultiplexer.Connect(config));
            });

            services.AddSingleton<IDistributedCache, GarnetDistributedCache>();
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}