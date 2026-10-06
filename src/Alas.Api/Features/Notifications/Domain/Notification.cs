using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Notifications.Domain;

public sealed class Notification
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public string Type { get; set; } = NotificationTypes.System;
    public DateTimeOffset? ReadAt { get; set; }
    public User? User { get; set; }
}