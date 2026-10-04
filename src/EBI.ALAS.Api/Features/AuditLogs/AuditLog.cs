namespace EBI.ALAS.Api.Features.AuditLogs;

public sealed class AuditLog
{
    public int Id { get; init; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? FromValue { get; set; }
    public string? ToValue { get; set; }
    public string? Summary { get; set; }
    public string? RawChanges { get; set; }
    public int UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}