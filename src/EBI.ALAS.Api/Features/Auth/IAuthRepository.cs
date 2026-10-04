using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Auth;

public interface IAuthRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken ct = default);
    Task UpdateAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllUserTokensAsync(int userId, string? reason, CancellationToken ct = default);
}

public interface ITokenRevocationRepository
{
    Task RevokeAsync(string jti, int userId, DateTime expiresAt, string? reason, CancellationToken ct = default);
    Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default);
    Task CleanupExpiredAsync(CancellationToken ct = default);
}