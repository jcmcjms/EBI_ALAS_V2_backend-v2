using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Features.Notifications;

public sealed class NotificationService(AppDbContext db) : INotificationService
{
    public async Task<IReadOnlyList<Notification>> GetRecentAsync(int userId, int limit, CancellationToken ct = default) =>
        await db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<int> CreateAsync(int userId, string title, string message, string type, string? link, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Link = link
        };
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(ct);
        return notification.Id;
    }

    public async Task CreateBatchAsync(IEnumerable<(int UserId, string Title, string Message, string? Link)> notifications, CancellationToken ct = default)
    {
        var entities = notifications.Select(n => new Notification
        {
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = "Info",
            Link = n.Link
        }).ToList();

        db.Notifications.AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    public async Task MarkAsReadAsync(int notificationId, int userId, CancellationToken ct = default)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct);
        if (notification is not null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default) =>
        await db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);
}

public sealed class RealtimeNotificationService : IRealtimeNotificationService
{
    public Task NotifyBranchAsync(string branchCode, string title, string message, string? link) => Task.CompletedTask;
    public Task NotifyUserAsync(int userId, string title, string message, string? link) => Task.CompletedTask;
    public Task NotifyDashboardUpdateAsync(string branchCode) => Task.CompletedTask;
}

public sealed class RemarkNotificationService : IRemarkNotificationService
{
    public Task NotifyDocumentRemarkAsync(int loanApplicationId, int documentChecklistId, string remarkType, string content, CancellationToken ct = default) => Task.CompletedTask;
}