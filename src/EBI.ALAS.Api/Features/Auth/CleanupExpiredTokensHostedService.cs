using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EBI.ALAS.Api.Features.Auth;

public sealed class CleanupExpiredTokensHostedService(
    IServiceProvider serviceProvider,
    ILogger<CleanupExpiredTokensHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var tokenRevocationRepo = scope.ServiceProvider.GetRequiredService<ITokenRevocationRepository>();
                var refreshTokenRepo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                await tokenRevocationRepo.CleanupExpiredAsync(stoppingToken);
                await refreshTokenRepo.RevokeAllUserTokensAsync(0, "Cleanup", stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up expired tokens");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}