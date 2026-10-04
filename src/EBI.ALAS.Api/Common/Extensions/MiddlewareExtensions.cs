using EBI.ALAS.Api.Shared.Middleware;

namespace EBI.ALAS.Api.Common.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Order matters - outermost middleware first
        app.UseCors("AllowFrontend");

        app.UseResponseCompression();

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.UseMiddleware<GlobalExceptionHandler>();

        app.UseMiddleware<RateLimitingMiddleware>();

        app.UseAuthentication();

        app.UseMiddleware<CsrfValidationMiddleware>();

        app.UseAuthorization();

        app.UseMiddleware<IdempotencyMiddleware>();

        app.UseOutputCache();

        return app;
    }
}