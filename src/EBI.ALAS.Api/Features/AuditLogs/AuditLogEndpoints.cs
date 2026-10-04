using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.AuditLogs;

public static class AuditLogEndpoints
{
    public static void MapAuditLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/audit-logs")
            .WithTags("AuditLogs")
            .RequireAuthorization();

        group.MapGet("/", GetAuditLogsAsync)
            .RequireAuthorization("CanViewAuditLogs")
            .WithName("GetAuditLogs")
            .Produces<ApiResponse<PagedResult<AuditLog>>>(200);
    }

    private static async Task<IResult> GetAuditLogsAsync(
        int? page, int? pageSize, string? entityType, int? entityId, string? action,
        int? userId, DateTime? fromDate, DateTime? toDate,
        IAuditLogService auditLogService, CancellationToken ct)
    {
        var p = Math.Max(page ?? 1, 1);
        var ps = Math.Clamp(pageSize ?? 50, 1, 200);

        var result = await auditLogService.GetPagedAsync(p, ps, entityType, entityId, action, userId, fromDate, toDate, ct);
        return TypedResults.Ok(ApiResponse<PagedResult<AuditLog>>.SuccessResponse(result));
    }
}