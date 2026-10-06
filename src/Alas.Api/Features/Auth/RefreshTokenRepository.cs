using Alas.Api.Features.Auth.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Auth;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public RefreshTokenRepository(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(int userId, string tokenHash, DateTimeOffset expiresAt, DateTimeOffset absoluteExpiresAt, string? deviceInfo, CancellationToken ct = default)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            AbsoluteExpiresAt = absoluteExpiresAt,
            DeviceInfo = deviceInfo,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(ct);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetActiveTokenByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        var now = _timeProvider.GetUtcNow();
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.IsRevoked && rt.ExpiresAt > now, ct);
    }

    public async Task RevokeTokenAsync(string tokenHash, CancellationToken ct = default)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);
        if (token is not null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeAllUserTokensAsync(int userId, CancellationToken ct = default)
    {
        await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true), ct);
    }

    public async Task<bool> IsTokenRevokedAsync(string tokenHash, CancellationToken ct = default) =>
        await _context.RefreshTokens.AnyAsync(rt => rt.TokenHash == tokenHash && rt.IsRevoked, ct);
}