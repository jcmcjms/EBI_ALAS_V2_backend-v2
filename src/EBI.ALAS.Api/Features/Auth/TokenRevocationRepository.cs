using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Features.Auth;

public sealed class TokenRevocationRepository(AppDbContext db) : ITokenRevocationRepository
{
    public async Task RevokeAsync(string jti, int userId, DateTime expiresAt, string? reason, CancellationToken ct = default)
    {
        var revoked = new RevokedToken
        {
            Jti = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            Reason = reason
        };
        db.RevokedTokens.Add(revoked);
        await db.SaveChangesAsync(ct);
    }

    public Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default) =>
        db.RevokedTokens.AsNoTracking().AnyAsync(t => t.Jti == jti, ct);

    public async Task CleanupExpiredAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var expired = await db.RevokedTokens
            .Where(t => t.ExpiresAt < now)
            .ToListAsync(ct);

        if (expired.Count > 0)
        {
            db.RevokedTokens.RemoveRange(expired);
            await db.SaveChangesAsync(ct);
        }
    }
}