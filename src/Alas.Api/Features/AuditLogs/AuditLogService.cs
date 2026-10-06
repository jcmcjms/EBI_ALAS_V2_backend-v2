namespace Alas.Api.Features.AuditLogs;

public sealed class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;
    private readonly TimeProvider _timeProvider;

    public AuditLogService(IAuditLogRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task LogAsync(
        int? userId,
        string userName,
        string action,
        string entityType,
        string entityId,
        string entityLabel,
        string summary,
        string? rawChanges = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        var entry = new AuditLog
        {
            Timestamp = _timeProvider.GetUtcNow(),
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            EntityLabel = entityLabel,
            Summary = summary,
            RawChanges = rawChanges,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _repository.LogAsync(entry, ct);
    }
}