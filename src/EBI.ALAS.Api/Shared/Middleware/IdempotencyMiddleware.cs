using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Features.Loans;
using EBI.ALAS.Api.Infrastructure.Data;

namespace EBI.ALAS.Api.Shared.Middleware;

public sealed class IdempotencyMiddleware(RequestDelegate next)
{
    private const string IdempotencyHeader = "Idempotency-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Method.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(IdempotencyHeader, out var idempotencyKeyHeader))
        {
            await next(context);
            return;
        }

        var idempotencyKey = idempotencyKeyHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out _))
        {
            await next(context);
            return;
        }

        context.Items["IdempotencyKey"] = idempotencyKey;

        await next(context);
    }
}