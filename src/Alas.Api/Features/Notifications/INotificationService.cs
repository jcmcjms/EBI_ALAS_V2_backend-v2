namespace Alas.Api.Features.Notifications;

public interface INotificationService
{
    Task CreateAsync(int userId, string title, string description, string? link = null, string? type = null, CancellationToken ct = default);
    Task CreateBatchAsync(IEnumerable<NotificationDraft> drafts, CancellationToken ct = default);
    Task<List<NotificationResponse>> GetUserNotificationsAsync(int userId, int limit = 20, CancellationToken ct = default);
    Task<InboxPage> GetInboxAsync(int userId, InboxQuery query, CancellationToken ct = default);
    Task<bool> MarkReadAsync(int userId, int notificationId, CancellationToken ct = default);
    Task<int> MarkAllReadAsync(int userId, CancellationToken ct = default);
    void AddNotificationForDeferredSave(int userId, string title, string description, string? link = null, string? type = null);
}