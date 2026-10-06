namespace Alas.Api.Features.AuditLogs;

public record AuditLogQueryParameters(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? Action = null,
    string? EntityType = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null);

public record AuditLogResponse(
    int Id,
    DateTimeOffset Timestamp,
    int? UserId,
    string UserName,
    string Action,
    string EntityType,
    string EntityId,
    string EntityLabel,
    string Summary,
    string? RawChanges,
    string? IpAddress,
    string? UserAgent);