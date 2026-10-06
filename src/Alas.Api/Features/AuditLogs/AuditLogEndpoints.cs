using Alas.Api.Composition;
using FluentValidation;

namespace Alas.Api.Features.AuditLogs;

public static class AuditLogEndpoints
{
    public static void MapAuditLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/audit-logs").WithTags("AuditLogs").RequireAuthorization();

        group.MapGet("/", HandleGetLogs)
            .WithName("GetAuditLogs")
            .ProducesValidationProblem();

        group.MapGet("/{id:int}", HandleGetById)
            .WithName("GetAuditLogById");
    }

    private static async Task<IResult> HandleGetLogs(
        [AsParameters] AuditLogQueryParameters parameters,
        IValidator<AuditLogQueryParameters> validator,
        IAuditLogRepository repository,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(parameters, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await repository.GetLogsAsync(parameters, ct);
        return Results.Ok(ApiResponse<PagedResult<AuditLogResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetById(
        int id,
        IAuditLogRepository repository,
        CancellationToken ct)
    {
        var log = await repository.GetByIdAsync(id, ct);
        return log is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<AuditLogResponse>.SuccessResponse(log));
    }
}