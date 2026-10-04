namespace EBI.ALAS.Api.Features.Notifications;

public sealed class Notification
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}