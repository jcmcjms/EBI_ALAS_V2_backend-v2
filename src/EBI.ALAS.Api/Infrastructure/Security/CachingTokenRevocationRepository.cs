using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using EBI.ALAS.Api.Features.Auth;
using EBI.ALAS.Api.Shared.Time;

namespace EBI.ALAS.Api.Infrastructure.Security;

public sealed class CachingTokenRevocationRepository(
    TokenRevocationRepository inner,
    IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    ITimeProvider timeProvider) : ITokenRevocationRepository
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

    public async Task RevokeAsync(string jti, int userId, DateTime expiresAt, string? reason, CancellationToken ct = default)
    {
        await inner.RevokeAsync(jti, userId, expiresAt, reason, ct);

        var cacheKey = $"revoked:{jti}";
        var ttl = expiresAt - timeProvider.UtcNow;
        if (ttl > TimeSpan.Zero)
        {
            memoryCache.Set(cacheKey, true, ttl);
            await distributedCache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }, ct);
        }
    }

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default)
    {
        var cacheKey = $"revoked:{jti}";

        if (memoryCache.TryGetValue(cacheKey, out bool cached) && cached)
        {
            return true;
        }

        var distributed = await distributedCache.GetStringAsync(cacheKey, ct);
        if (distributed != null)
        {
            memoryCache.Set(cacheKey, true, CacheTtl);
            return true;
        }

        var isRevoked = await inner.IsRevokedAsync(jti, ct);
        if (isRevoked)
        {
            memoryCache.Set(cacheKey, true, CacheTtl);
        }
        return isRevoked;
    }

    public Task CleanupExpiredAsync(CancellationToken ct = default) => inner.CleanupExpiredAsync(ct);
}