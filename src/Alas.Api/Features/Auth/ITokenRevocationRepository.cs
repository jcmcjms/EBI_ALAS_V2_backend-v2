namespace Alas.Api.Features.Auth;

public interface ITokenRevocationRepository
{
    Task RevokeTokenAsync(string tokenId, int userId, DateTimeOffset expiresAt, CancellationToken ct = default);
    Task<bool> IsTokenRevokedAsync(string tokenId, CancellationToken ct = default);
}