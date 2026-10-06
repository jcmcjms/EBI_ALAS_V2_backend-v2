using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Alas.Api.Features.Auth.Domain;
using Alas.Api.Features.Users;
using Alas.Api.Features.Users.Domain;
using Microsoft.Extensions.Options;

namespace Alas.Api.Features.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenRevocationRepository tokenRevocationRepository,
    IOptions<JwtSettings> jwtOptions,
    TimeProvider timeProvider,
    ILogger<AuthService> logger) : IAuthService
{
    private static readonly string PrecomputedDummyHash = BCrypt.Net.BCrypt.HashPassword("dummy_password");
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<AuthResult> LoginAsync(LoginRequest request, HttpContext http, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userRepository.GetUserByUsernameAsync(request.Username, ct);
        var passwordHash = user?.PasswordHash ?? PrecomputedDummyHash;
        var isPasswordValid = passwordHasher.VerifyPassword(request.Password, passwordHash);

        if (user is null || !isPasswordValid || !user.IsActive)
        {
            logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
            return AuthResult.FailureResult("Invalid credentials");
        }

        if (user.TempPasswordExpiresAt is { } expires && timeProvider.GetUtcNow() > expires)
        {
            logger.LogWarning("Expired temporary password for username: {Username}", request.Username);
            return AuthResult.FailureResult("Temporary password expired. Contact your administrator for a new credential.");
        }

        var rawRefreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = jwtTokenService.HashRefreshToken(rawRefreshToken);
        var now = timeProvider.GetUtcNow();
        var refreshExpiry = now.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        var absoluteExpiry = now.AddDays(_jwtSettings.AbsoluteSessionExpiryDays);
        var deviceInfo = GetDeviceInfo(http);

        var refreshToken = await refreshTokenRepository.CreateRefreshTokenAsync(
            user.Id, refreshTokenHash, refreshExpiry, absoluteExpiry, deviceInfo, ct);

        var (accessToken, xsrfToken) = jwtTokenService.GenerateTokenWithXsrf(user, refreshToken.Id);
        var accessExpiresAt = now.AddMinutes(_jwtSettings.ExpiryMinutes);

        logger.LogInformation("User {Username} logged in successfully", user.Username);

        return AuthResult.SuccessResult(
            new LoginResponse { AccessToken = accessToken, ExpiresAt = accessExpiresAt },
            rawRefreshToken,
            refreshExpiry,
            xsrfToken,
            accessExpiresAt);
    }

    public async Task<AuthResult> RefreshAsync(HttpContext http, CancellationToken ct = default)
    {
        if (!http.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var rawRefreshToken) || string.IsNullOrEmpty(rawRefreshToken))
        {
            logger.LogDebug("Refresh endpoint called without refresh token cookie");
            return AuthResult.FailureResult("No refresh token provided");
        }

        var tokenHash = jwtTokenService.HashRefreshToken(rawRefreshToken);
        var storedToken = await refreshTokenRepository.GetActiveTokenByHashAsync(tokenHash, ct);

        if (storedToken is null)
        {
            var isRevokedReuse = await refreshTokenRepository.IsTokenRevokedAsync(tokenHash, ct);
            if (isRevokedReuse)
            {
                logger.LogWarning("SECURITY: Refresh token reuse detected. Revoking all user tokens as a theft signal.");
                return AuthResult.FailureResult("Invalid refresh token");
            }

            logger.LogWarning("Refresh token not found, revoked, or expired");
            return AuthResult.FailureResult("Invalid refresh token");
        }

        var user = await userRepository.GetUserByIdAsync(storedToken.UserId, ct);
        if (user is null || !user.IsActive)
        {
            logger.LogWarning("Refresh token belongs to inactive or missing user {UserId}", storedToken.UserId);
            return AuthResult.FailureResult("User not found or inactive");
        }

        var now = timeProvider.GetUtcNow();
        var currentJti = http.User?.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (!string.IsNullOrEmpty(currentJti) && int.TryParse(http.User?.FindFirstValue("userId"), out var currentUserId))
        {
            var currentExpiry = now.AddMinutes(_jwtSettings.ExpiryMinutes);
            await tokenRevocationRepository.RevokeTokenAsync(currentJti, currentUserId, currentExpiry, ct);
        }

        var newDeviceInfo = GetDeviceInfo(http);
        var newRawRefreshToken = jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenHash = jwtTokenService.HashRefreshToken(newRawRefreshToken);
        var newRefreshExpiry = now.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        var newAbsoluteExpiry = now.AddDays(_jwtSettings.AbsoluteSessionExpiryDays);

        var newRefreshToken = await refreshTokenRepository.CreateRefreshTokenAsync(
            user.Id, newRefreshTokenHash, newRefreshExpiry, newAbsoluteExpiry, newDeviceInfo, ct);

        await refreshTokenRepository.RevokeTokenAsync(tokenHash, ct);

        var (newAccessToken, newXsrfToken) = jwtTokenService.GenerateTokenWithXsrf(user, newRefreshToken.Id);
        var newAccessExpiresAt = now.AddMinutes(_jwtSettings.ExpiryMinutes);

        logger.LogInformation("Token refreshed for user {UserId}", user.Id);

        return AuthResult.SuccessResult(
            new LoginResponse { AccessToken = newAccessToken, ExpiresAt = newAccessExpiresAt },
            newRawRefreshToken,
            newRefreshExpiry,
            newXsrfToken,
            newAccessExpiresAt);
    }

    public async Task LogoutAsync(ClaimsPrincipal principal, HttpContext http, CancellationToken ct = default)
    {
        var tokenId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var userIdClaim = principal.FindFirstValue("userId");
        _ = int.TryParse(userIdClaim, out var userId);

        if (!string.IsNullOrEmpty(tokenId) && userId > 0)
        {
            var expiresAt = timeProvider.GetUtcNow().AddMinutes(_jwtSettings.ExpiryMinutes);
            await tokenRevocationRepository.RevokeTokenAsync(tokenId, userId, expiresAt, ct);
            logger.LogInformation("Access token {TokenId} revoked for user {UserId}", tokenId, userId);
        }

        if (http.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenCookieName, out var rawRefreshToken) && !string.IsNullOrEmpty(rawRefreshToken))
        {
            var tokenHash = jwtTokenService.HashRefreshToken(rawRefreshToken);
            await refreshTokenRepository.RevokeTokenAsync(tokenHash, ct);
            logger.LogInformation("Refresh token revoked for user {UserId}", userIdClaim ?? "unknown");
        }
    }

    public async Task ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordRequest request, HttpContext http, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = principal.FindFirstValue("userId");
        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var user = await userRepository.GetUserByIdAsync(userId, ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("User not found or inactive");

        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect");

        if (request.CurrentPassword == request.NewPassword)
            throw new InvalidOperationException("New password must be different from current password");

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.MustChangePassword = false;
        user.TempPasswordExpiresAt = null;
        await userRepository.UpdateUserAsync(ct);

        await refreshTokenRepository.RevokeAllUserTokensAsync(userId, ct);

        var tokenId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (!string.IsNullOrEmpty(tokenId))
        {
            var expiresAt = timeProvider.GetUtcNow().AddMinutes(_jwtSettings.ExpiryMinutes);
            await tokenRevocationRepository.RevokeTokenAsync(tokenId, userId, expiresAt, ct);
        }

        logger.LogInformation("User {UserId} changed password successfully", userId);
    }

    public async Task<MeResponse?> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken ct = default)
    {
        var userIdClaim = principal.FindFirstValue("userId");
        if (!int.TryParse(userIdClaim, out var userId))
            return null;

        var user = await userRepository.GetUserByIdAsync(userId, ct);
        if (user is null || !user.IsActive)
            return null;

        var permissions = principal.FindAll("permission").Select(c => c.Value).ToArray();
        var roles = new[] { user.Role }.Where(r => !string.IsNullOrEmpty(r)).ToArray();

        return new MeResponse(
            user.Id,
            user.Username,
            user.Email ?? string.Empty,
            $"{user.FirstName} {user.LastName}".Trim(),
            roles,
            permissions,
            user.IsActive,
            user.MustChangePassword,
            user.BranchId);
    }

    private static string GetDeviceInfo(HttpContext http)
    {
        var userAgent = http.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent)) return "Unknown Device";
        return userAgent.Length > 500 ? userAgent[..500] : userAgent;
    }
}