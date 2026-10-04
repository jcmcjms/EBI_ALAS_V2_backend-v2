using System.Security.Claims;

namespace EBI.ALAS.Api.Features.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<Claim>? additionalClaims = null);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    string? GetJtiFromToken(string token);
    DateTime GetTokenExpiry(string token);
}

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<RefreshResponse>> RefreshAsync(string refreshTokenValue, CancellationToken ct = default);
    Task<Result> LogoutAsync(string? jti, int userId, DateTime? accessTokenExpiry, CancellationToken ct = default);
    Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
}