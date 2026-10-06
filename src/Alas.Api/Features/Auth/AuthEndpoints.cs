using System.Security.Claims;
using Alas.Api.Composition;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Alas.Api.Features.Auth;

public static class AuthEndpoints
{
    private const string XsrfCookieName = "XSRF-TOKEN";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", HandleLogin)
            .WithName("Login")
            .RequireRateLimiting("LoginLimiter")
            .Produces<ApiResponse<LoginResponse>>(200)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        group.MapPost("/refresh", HandleRefresh)
            .WithName("RefreshToken")
            .RequireRateLimiting("LoginLimiter")
            .Produces<ApiResponse<LoginResponse>>(200)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", HandleLogout)
            .WithName("Logout")
            .Produces<ApiResponse>(200)
            .RequireAuthorization();

        group.MapPost("/change-password", HandleChangePassword)
            .WithName("ChangePassword")
            .Produces<ApiResponse>(200)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        group.MapGet("/me", HandleMe)
            .WithName("GetCurrentUser")
            .Produces<ApiResponse<MeResponse>>(200)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleLogin(
        LoginRequest request,
        IValidator<LoginRequest> validator,
        IAuthService authService,
        HttpContext http,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await authService.LoginAsync(request, http, ct);
        if (!result.Success)
            return Results.Problem(
                title: "Unauthorized",
                detail: "Invalid username or password.",
                statusCode: StatusCodes.Status401Unauthorized,
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.2");

        SetRefreshTokenCookie(http, result.RefreshToken!, result.RefreshTokenExpiry!.Value);
        SetXsrfCookie(http, result.XsrfToken!, result.AccessTokenExpiry!.Value);

        return Results.Ok(ApiResponse<LoginResponse>.SuccessResponse(result.Response!, "Login successful"));
    }

    private static async Task<IResult> HandleRefresh(
        HttpContext http,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RefreshAsync(http, ct);
        if (!result.Success)
            return Results.Problem(
                title: "Unauthorized",
                detail: "Session expired or invalid. Please log in again.",
                statusCode: StatusCodes.Status401Unauthorized,
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.2");

        SetRefreshTokenCookie(http, result.RefreshToken!, result.RefreshTokenExpiry!.Value);
        SetXsrfCookie(http, result.XsrfToken!, result.AccessTokenExpiry!.Value);

        return Results.Ok(ApiResponse<LoginResponse>.SuccessResponse(result.Response!, "Token refreshed successfully"));
    }

    private static async Task<IResult> HandleLogout(
        ClaimsPrincipal principal,
        HttpContext http,
        IAuthService authService,
        CancellationToken ct)
    {
        await authService.LogoutAsync(principal, http, ct);

        http.Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth"
        });

        return Results.Ok(ApiResponse.SuccessResponse("Logged out successfully"));
    }

    private static async Task<IResult> HandleChangePassword(
        ClaimsPrincipal principal,
        ChangePasswordRequest request,
        IValidator<ChangePasswordRequest> validator,
        IAuthService authService,
        HttpContext http,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            await authService.ChangePasswordAsync(principal, request, http, ct);

            http.Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            });

            return Results.Ok(ApiResponse.SuccessResponse("Password changed successfully. Please log in again."));
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Problem(
                title: "Unauthorized",
                detail: "Session expired or invalid. Please log in again.",
                statusCode: StatusCodes.Status401Unauthorized,
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.2");
        }
        catch (InvalidOperationException ex)
        {
            return Results.Problem(
                title: "Bad Request",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.1");
        }
    }

    private static async Task<IResult> HandleMe(
        ClaimsPrincipal principal,
        IAuthService authService,
        CancellationToken ct)
    {
        var me = await authService.GetCurrentUserAsync(principal, ct);
        return me is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<MeResponse>.SuccessResponse(me));
    }

    private static void SetRefreshTokenCookie(HttpContext http, string refreshToken, DateTimeOffset expiry)
    {
        var isLocal = http.Request.IsLocalRequest();
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isLocal,
            SameSite = isLocal ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = expiry,
            Path = "/api/auth",
            IsEssential = true
        };

        if (isLocal)
            cookieOptions.Domain = "localhost";

        http.Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private static void SetXsrfCookie(HttpContext http, string xsrfToken, DateTimeOffset expiry)
    {
        var isLocal = http.Request.IsLocalRequest();
        var cookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = !isLocal,
            SameSite = isLocal ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = expiry,
            Path = "/",
            IsEssential = true
        };

        if (isLocal)
            cookieOptions.Domain = "localhost";

        http.Response.Cookies.Append(XsrfCookieName, xsrfToken, cookieOptions);
    }
}

internal static class HttpRequestExtensions
{
    public static bool IsLocalRequest(this HttpRequest request)
        => request.Host.Host is "localhost" or "127.0.0.1";
}