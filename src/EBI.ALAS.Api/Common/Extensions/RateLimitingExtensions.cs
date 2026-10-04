using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace EBI.ALAS.Api.Common.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddBankingRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter("LoginPolicy", opts =>
            {
                opts.PermitLimit = 5;
                opts.Window = TimeSpan.FromSeconds(60);
                opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opts.QueueLimit = 0;
            });

            options.AddFixedWindowLimiter("GlobalPolicy", opts =>
            {
                opts.PermitLimit = 120;
                opts.Window = TimeSpan.FromSeconds(60);
                opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opts.QueueLimit = 10;
            });
        });

        return services;
    }
}