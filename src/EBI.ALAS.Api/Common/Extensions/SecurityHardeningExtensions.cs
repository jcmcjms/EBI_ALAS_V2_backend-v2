using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EBI.ALAS.Api.Infrastructure.Security;

namespace EBI.ALAS.Api.Common.Extensions;

public static class SecurityHardeningExtensions
{
    public static IServiceCollection AddBankingSecurityHardening(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // IP Allowlist for admin endpoints (optional)
        var adminAllowedIps = configuration.GetSection("Security:AdminAllowedIps").Get<string[]>();
        if (adminAllowedIps?.Length > 0)
        {
            services.AddSingleton(new IpAllowlistOptions { AllowedIps = adminAllowedIps.ToHashSet() });
        }

        // Security validator for additional checks
        services.AddScoped<BankingSecurityValidator>();

        return services;
    }
}