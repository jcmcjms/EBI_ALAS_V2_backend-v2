using System.Security.Claims;

namespace Alas.Api.Features.Auth;

public static class AuthConstants
{
    public const string RefreshTokenCookieName = "refreshToken";
}

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request, HttpContext http, CancellationToken ct = default);
    Task<AuthResult> RefreshAsync(HttpContext http, CancellationToken ct = default);
    Task LogoutAsync(ClaimsPrincipal principal, HttpContext http, CancellationToken ct = default);
    Task ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordRequest request, HttpContext http, CancellationToken ct = default);
    Task<MeResponse?> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken ct = default);
}

public sealed record MeResponse(
    int Id,
    string Username,
    string Email,
    string FullName,
    string[] Roles,
    string[] Permissions,
    bool IsActive,
    bool MustChangePassword,
    string BranchId);

public sealed class AuthResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public LoginResponse? Response { get; init; }
    public string? RefreshToken { get; init; }
    public DateTimeOffset? RefreshTokenExpiry { get; init; }
    public string? XsrfToken { get; init; }
    public DateTimeOffset? AccessTokenExpiry { get; init; }

    public static AuthResult SuccessResult(LoginResponse response, string refreshToken, DateTimeOffset refreshTokenExpiry, string xsrfToken, DateTimeOffset accessTokenExpiry) =>
        new()
        {
            Success = true,
            Response = response,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry,
            XsrfToken = xsrfToken,
            AccessTokenExpiry = accessTokenExpiry
        };

    public static AuthResult FailureResult(string error) =>
        new() { Success = false, Error = error };
}