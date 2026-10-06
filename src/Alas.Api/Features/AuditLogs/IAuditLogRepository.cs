using Alas.Api.Composition;

namespace Alas.Api.Features.AuditLogs;

public interface IAuditLogRepository
{
    Task LogAsync(AuditLog entry, CancellationToken ct = default);
    Task<PagedResult<AuditLogResponse>> GetLogsAsync(AuditLogQueryParameters parameters, CancellationToken ct = default);
    Task<AuditLogResponse?> GetByIdAsync(int id, CancellationToken ct = default);
}