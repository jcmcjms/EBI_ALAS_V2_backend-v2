namespace EBI.ALAS.Api.Shared.Middleware;

public sealed class CsrfValidationMiddleware(RequestDelegate next, ILogger<CsrfValidationMiddleware> logger)
{
    private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "GET", "HEAD", "OPTIONS", "TRACE"
    };

    private const string CsrfHeader = "X-CSRF-Token";

    public async Task InvokeAsync(HttpContext context)
    {
        if (SafeMethods.Contains(context.Request.Method))
        {
            await next(context);
            return;
        }

        // Skip CSRF for login, refresh, and logout endpoints
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (path is not null && (
            path.Contains("/api/auth/login") ||
            path.Contains("/api/auth/refresh") ||
            path.Contains("/api/auth/logout")))
        {
            await next(context);
            return;
        }

        var csrfToken = context.Request.Headers[CsrfHeader].FirstOrDefault();
        var cookieToken = context.Request.Cookies["csrf_token"];

        if (string.IsNullOrEmpty(csrfToken) || string.IsNullOrEmpty(cookieToken) ||
            !string.Equals(csrfToken, cookieToken, StringComparison.Ordinal))
        {
            logger.LogWarning("CSRF validation failed | Path: {Path} | HasHeader: {HasHeader} | HasCookie: {HasCookie}",
                path, !string.IsNullOrEmpty(csrfToken), !string.IsNullOrEmpty(cookieToken));

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "CSRF validation failed", code = "CSRF_INVALID" });
            return;
        }

        await next(context);
    }
}