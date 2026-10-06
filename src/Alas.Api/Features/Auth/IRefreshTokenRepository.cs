using Alas.Api.Features.Auth.Domain;

namespace Alas.Api.Features.Auth;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateRefreshTokenAsync(int userId, string tokenHash, DateTimeOffset expiresAt, DateTimeOffset absoluteExpiresAt, string? deviceInfo, CancellationToken ct = default);
    Task<RefreshToken?> GetActiveTokenByHashAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeTokenAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeAllUserTokensAsync(int userId, CancellationToken ct = default);
    Task<bool> IsTokenRevokedAsync(string tokenHash, CancellationToken ct = default);
}