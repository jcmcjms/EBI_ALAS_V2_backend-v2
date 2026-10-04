using System.Text.Json;
using EBI.ALAS.Api.Shared.Errors;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Shared.Middleware;

public sealed class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Response.Headers["X-Correlation-ID"].FirstOrDefault() ?? "unknown";

        logger.LogError(exception,
            "Unhandled exception | CorrelationId: {CorrelationId} | Path: {Path}",
            correlationId, context.Request.Path);

        var (statusCode, errorCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND", exception.Message),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "FORBIDDEN", exception.Message),
            InvalidWorkflowException wf => (StatusCodes.Status400BadRequest, "INVALID_WORKFLOW", wf.Message),
            CapacityGateException => (StatusCodes.Status400BadRequest, "CAPACITY_GATE", exception.Message),
            FluentValidation.ValidationException ve => (StatusCodes.Status400BadRequest, "VALIDATION_ERROR",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Authentication required"),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR",
                env.IsDevelopment() ? exception.Message : "An unexpected error occurred")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Errors = [errorCode]
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}