using Alas.Api.Composition;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.AuditLogs;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context) => _context = context;

    public async Task LogAsync(AuditLog entry, CancellationToken ct = default)
    {
        _context.AuditLogs.Add(entry);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<AuditLogResponse>> GetLogsAsync(AuditLogQueryParameters parameters, CancellationToken ct = default)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search;
            query = query.Where(x =>
                x.UserName.Contains(search) ||
                x.EntityLabel.Contains(search) ||
                x.Summary.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Action))
            query = query.Where(x => x.Action == parameters.Action);

        if (!string.IsNullOrWhiteSpace(parameters.EntityType))
            query = query.Where(x => x.EntityType == parameters.EntityType);

        if (parameters.StartDate.HasValue)
            query = query.Where(x => x.Timestamp >= parameters.StartDate.Value);

        if (parameters.EndDate.HasValue)
            query = query.Where(x => x.Timestamp <= parameters.EndDate.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(x => new AuditLogResponse(
                x.Id, x.Timestamp, x.UserId, x.UserName, x.Action,
                x.EntityType, x.EntityId, x.EntityLabel, x.Summary,
                x.RawChanges, x.IpAddress, x.UserAgent))
            .ToListAsync(ct);

        return new PagedResult<AuditLogResponse>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<AuditLogResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AuditLogResponse(
                x.Id, x.Timestamp, x.UserId, x.UserName, x.Action,
                x.EntityType, x.EntityId, x.EntityLabel, x.Summary,
                x.RawChanges, x.IpAddress, x.UserAgent))
            .FirstOrDefaultAsync(ct);
    }
}