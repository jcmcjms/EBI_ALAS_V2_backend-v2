using Alas.Api.Features.Notifications.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public NotificationService(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task CreateAsync(int userId, string title, string description, string? link = null, string? type = null, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Description = description,
            Link = link,
            CreatedAt = _timeProvider.GetUtcNow(),
            Type = type ?? NotificationTypes.System
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(ct);
    }

    public async Task CreateBatchAsync(IEnumerable<NotificationDraft> drafts, CancellationToken ct = default)
    {
        var now = _timeProvider.GetUtcNow();
        var entities = drafts.Select(d => new Notification
        {
            UserId = d.UserId,
            Title = d.Title,
            Description = d.Description,
            Link = d.Link,
            CreatedAt = now,
            Type = d.Type ?? NotificationTypes.System
        }).ToList();

        if (entities.Count == 0) return;

        _context.Notifications.AddRange(entities);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<NotificationResponse>> GetUserNotificationsAsync(int userId, int limit = 20, CancellationToken ct = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .Select(n => new NotificationResponse(
                n.Id, n.Title, n.Description, n.Link,
                n.IsRead, n.CreatedAt, n.Type, n.ReadAt))
            .ToListAsync(ct);
    }

    public async Task<InboxPage> GetInboxAsync(int userId, InboxQuery query, CancellationToken ct = default)
    {
        var q = _context.Notifications.AsNoTracking().Where(n => n.UserId == userId);

        q = query.Status switch
        {
            "unread" => q.Where(n => !n.IsRead),
            "read" => q.Where(n => n.IsRead),
            _ => q,
        };

        if (!string.IsNullOrWhiteSpace(query.Type))
            q = q.Where(n => n.Type == query.Type);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = EscapeLike(query.Search.Trim());
            q = q.Where(n =>
                EF.Functions.Like(n.Title, $"%{s}%") ||
                EF.Functions.Like(n.Description, $"%{s}%"));
        }

        var unreadCount = await _context.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync(ct);

        var totalCount = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(n => new NotificationResponse(
                n.Id, n.Title, n.Description, n.Link,
                n.IsRead, n.CreatedAt, n.Type, n.ReadAt))
            .ToListAsync(ct);

        return new InboxPage(items, totalCount, unreadCount);
    }

    public async Task<bool> MarkReadAsync(int userId, int notificationId, CancellationToken ct = default)
    {
        var row = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct);

        if (row is null) return false;

        if (!row.IsRead)
        {
            row.IsRead = true;
            row.ReadAt = _timeProvider.GetUtcNow();
            await _context.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task<int> MarkAllReadAsync(int userId, CancellationToken ct = default)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(ct);

        if (unread.Count == 0) return 0;

        var now = _timeProvider.GetUtcNow();
        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = now;
        }

        await _context.SaveChangesAsync(ct);
        return unread.Count;
    }

    public void AddNotificationForDeferredSave(int userId, string title, string description, string? link = null, string? type = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Description = description,
            Link = link,
            CreatedAt = _timeProvider.GetUtcNow(),
            Type = type ?? NotificationTypes.System
        };
        _context.Notifications.Add(notification);
    }

    private static string EscapeLike(string input) =>
        input.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
}