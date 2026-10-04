namespace EBI.ALAS.Api.Shared.Middleware;

public sealed class RateLimitingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Rate limiting is handled by the built-in middleware registered via AddRateLimiter
        // This middleware is kept for compatibility but does nothing
        await next(context);
    }
}