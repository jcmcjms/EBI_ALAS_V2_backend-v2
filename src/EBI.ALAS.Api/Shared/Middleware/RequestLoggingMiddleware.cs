using System.Diagnostics;
using System.Text.Json;

namespace EBI.ALAS.Api.Shared.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var correlationId = context.Response.Headers["X-Correlation-ID"].FirstOrDefault() ?? "unknown";

        var requestInfo = new
        {
            Method = context.Request.Method,
            Path = context.Request.Path.Value,
            QueryString = context.Request.QueryString.Value,
            UserId = context.User.FindFirst("uid")?.Value ?? "anonymous",
            IpAddress = context.Connection.RemoteIpAddress?.ToString()
        };

        logger.LogInformation("HTTP {Method} {Path} started | CorrelationId: {CorrelationId} | User: {UserId}",
            requestInfo.Method, requestInfo.Path, correlationId, requestInfo.UserId);

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            var statusCode = context.Response.StatusCode;

            logger.LogInformation(
                "HTTP {Method} {Path} completed {StatusCode} in {ElapsedMs}ms | CorrelationId: {CorrelationId}",
                requestInfo.Method, requestInfo.Path, statusCode, sw.ElapsedMilliseconds, correlationId);
        }
    }
}