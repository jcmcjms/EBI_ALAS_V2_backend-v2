using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.AuditLogs;

public interface IAuditLogService
{
    Task<PagedResult<AuditLog>> GetPagedAsync(
        int page, int pageSize, string? entityType, int? entityId, string? action,
        int? userId, DateTime? fromDate, DateTime? toDate, CancellationToken ct = default);
}