using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .DisableAntiforgery();

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("Login")
            .Produces<ApiResponse<LoginResponse>>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(401);

        group.MapPost("/refresh", RefreshAsync)
            .AllowAnonymous()
            .WithName("RefreshToken")
            .Produces<ApiResponse<RefreshResponse>>(200)
            .Produces<ApiResponse>(401);

        group.MapPost("/logout", LogoutAsync)
            .RequireAuthorization()
            .WithName("Logout")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(401);

        group.MapPost("/change-password", ChangePasswordAsync)
            .RequireAuthorization()
            .WithName("ChangePassword")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(401);
    }

    private static async Task<Results<Ok<ApiResponse<LoginResponse>>, BadRequest<ApiResponse<LoginResponse>>, UnauthorizedHttpResult>> LoginAsync(
        LoginRequest request,
        IAuthService authService,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        HttpContext httpContext,
        IOptions<JwtOptions> jwtOptions,
        CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(ApiResponse<LoginResponse>.FailureResponse(result.Error.Message, result.Error.Code));
        }

        var refreshTokenValue = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = passwordHasher.HashPassword(refreshTokenValue);

        // Set refresh token as HttpOnly cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            Path = "/"
        };
        httpContext.Response.Cookies.Append("refresh_token", refreshTokenValue, cookieOptions);

        return TypedResults.Ok(ApiResponse<LoginResponse>.SuccessResponse(result.Value!, "Login successful"));
    }

    private static async Task<Results<Ok<ApiResponse<RefreshResponse>>, UnauthorizedHttpResult>> RefreshAsync(
        HttpContext httpContext,
        IAuthService authService,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        IOptions<JwtOptions> jwtOptions,
        CancellationToken ct)
    {
        if (!httpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshTokenValue))
        {
            return TypedResults.Unauthorized();
        }

        var result = await authService.RefreshAsync(refreshTokenValue, ct);
        if (result.IsFailure)
        {
            // Clear the invalid cookie
            httpContext.Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = !httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
            return TypedResults.Unauthorized();
        }

        // Set new refresh token cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            Path = "/"
        };
        httpContext.Response.Cookies.Append("refresh_token", jwtTokenService.GenerateRefreshToken(), cookieOptions);

        return TypedResults.Ok(ApiResponse<RefreshResponse>.SuccessResponse(result.Value!, "Token refreshed"));
    }

    private static async Task<Results<Ok<ApiResponse>, UnauthorizedHttpResult>> LogoutAsync(
        HttpContext httpContext,
        IAuthService authService,
        IJwtTokenService jwtTokenService,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userId = user.GetUserId();
        var jti = user.FindFirst("jti")?.Value;
        var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "") ?? "";
        var accessTokenExpiry = string.IsNullOrEmpty(authHeader) ? (DateTime?)null : jwtTokenService.GetTokenExpiry(authHeader);

        await authService.LogoutAsync(jti, userId, accessTokenExpiry, ct);

        // Clear refresh token cookie
        httpContext.Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return TypedResults.Ok(ApiResponse.SuccessResponse("Logout successful"));
    }

    private static async Task<Results<Ok<ApiResponse>, BadRequest<ApiResponse>, UnauthorizedHttpResult>> ChangePasswordAsync(
        ChangePasswordRequest request,
        IAuthService authService,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await authService.ChangePasswordAsync(userId, request, ct);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(ApiResponse.FailureResponse(result.Error.Message, result.Error.Code));
        }

        return TypedResults.Ok(ApiResponse.SuccessResponse("Password changed successfully. Please login again."));
    }
}