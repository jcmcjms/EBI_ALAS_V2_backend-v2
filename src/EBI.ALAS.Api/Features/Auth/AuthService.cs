using System.Security.Claims;
using EBI.ALAS.Api.Features.Auth;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Auth;

public sealed class AuthService(
    IAuthRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenRevocationRepository tokenRevocationRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ITimeProvider timeProvider) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, ct);
        if (user is null || !user.IsActive)
        {
            // Timing attack mitigation: always verify even for non-existent users
            passwordHasher.VerifyPassword(request.Password, "$2a$11$dummyhashfordummyuserthatdoesnotexist");
            return Result<LoginResponse>.Failure("AUTH_INVALID_CREDENTIALS", "Invalid username or password");
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure("AUTH_INVALID_CREDENTIALS", "Invalid username or password");
        }

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var refreshTokenValue = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = passwordHasher.HashPassword(refreshTokenValue);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = timeProvider.UtcNow.AddDays(7)
        };

        await refreshTokenRepository.CreateAsync(refreshToken, ct);

        user.LastLoginAt = timeProvider.UtcNow;
        await userRepository.UpdateAsync(user, ct);

        var accessTokenExpiry = jwtTokenService.GetTokenExpiry(accessToken);

        var response = new LoginResponse(
            accessToken,
            accessTokenExpiry,
            new UserDto(
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.MiddleName,
                user.LastName,
                user.Suffix,
                user.Role,
                user.BranchCode,
                user.JobTitle,
                user.MustChangePassword));

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<RefreshResponse>> RefreshAsync(string refreshTokenValue, CancellationToken ct = default)
    {
        var refreshTokenHash = passwordHasher.HashPassword(refreshTokenValue);
        var storedToken = await refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, ct);

        if (storedToken is null || !storedToken.IsActive)
        {
            return Result<RefreshResponse>.Failure("AUTH_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token");
        }

        var user = await userRepository.GetByIdAsync(storedToken.UserId, ct);
        if (user is null || !user.IsActive)
        {
            return Result<RefreshResponse>.Failure("AUTH_USER_INACTIVE", "User not found or inactive");
        }

        // Revoke old refresh token
        storedToken.RevokedAt = timeProvider.UtcNow;
        storedToken.RevokedReason = "Rotated";
        await refreshTokenRepository.UpdateAsync(storedToken, ct);

        // Create new refresh token
        var newRefreshTokenValue = jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenHash = passwordHasher.HashPassword(newRefreshTokenValue);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = timeProvider.UtcNow.AddDays(7)
        };

        await refreshTokenRepository.CreateAsync(newRefreshToken, ct);

        // Generate new access token
        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var accessTokenExpiry = jwtTokenService.GetTokenExpiry(accessToken);

        var response = new RefreshResponse(accessToken, accessTokenExpiry);
        return Result<RefreshResponse>.Success(response);
    }

    public async Task<Result> LogoutAsync(string? jti, int userId, DateTime? accessTokenExpiry, CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(jti) && accessTokenExpiry.HasValue)
        {
            await tokenRevocationRepository.RevokeAsync(jti, userId, accessTokenExpiry.Value, "Logout", ct);
        }

        await refreshTokenRepository.RevokeAllUserTokensAsync(userId, "Logout", ct);
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return Result.Failure("AUTH_USER_NOT_FOUND", "User not found");
        }

        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure("AUTH_INVALID_CURRENT_PASSWORD", "Current password is incorrect");
        }

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.MustChangePassword = false;
        user.PasswordChangedAt = timeProvider.UtcNow;

        await userRepository.UpdateAsync(user, ct);
        await refreshTokenRepository.RevokeAllUserTokensAsync(userId, "Password changed", ct);

        return Result.Success();
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        userRepository.GetByIdAsync(id, ct);
}