using Alas.Api.Features.Auth.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Auth;

public sealed class TokenRevocationRepository : ITokenRevocationRepository
{
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public TokenRevocationRepository(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task RevokeTokenAsync(string tokenId, int userId, DateTimeOffset expiresAt, CancellationToken ct = default)
    {
        var revokedToken = new RevokedToken
        {
            TokenId = tokenId,
            UserId = userId,
            ExpiresAt = expiresAt,
            RevokedAt = _timeProvider.GetUtcNow()
        };

        _context.RevokedTokens.Add(revokedToken);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> IsTokenRevokedAsync(string tokenId, CancellationToken ct = default) =>
        await _context.RevokedTokens.AnyAsync(rt => rt.TokenId == tokenId, ct);
}