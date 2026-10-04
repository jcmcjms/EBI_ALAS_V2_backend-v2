using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.AuditLogs;

public sealed class AuditLogService(AppDbContext db) : IAuditLogService
{
    public async Task<PagedResult<AuditLog>> GetPagedAsync(
        int page, int pageSize, string? entityType, int? entityId, string? action,
        int? userId, DateTime? fromDate, DateTime? toDate, CancellationToken ct = default)
    {
        var query = db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        if (entityId.HasValue)
        {
            query = query.Where(a => a.EntityId == entityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= toDate.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<AuditLog>(items, totalCount, page, pageSize);
    }
}