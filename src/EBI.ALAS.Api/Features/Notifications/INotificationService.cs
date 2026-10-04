namespace EBI.ALAS.Api.Features.Notifications;

public interface INotificationService
{
    Task<IReadOnlyList<Notification>> GetRecentAsync(int userId, int limit, CancellationToken ct = default);
    Task<int> CreateAsync(int userId, string title, string message, string type, string? link, CancellationToken ct = default);
    Task CreateBatchAsync(IEnumerable<(int UserId, string Title, string Message, string? Link)> notifications, CancellationToken ct = default);
    Task MarkAsReadAsync(int notificationId, int userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
}

public interface IRealtimeNotificationService
{
    Task NotifyBranchAsync(string branchCode, string title, string message, string? link);
    Task NotifyUserAsync(int userId, string title, string message, string? link);
    Task NotifyDashboardUpdateAsync(string branchCode);
}

public interface IRemarkNotificationService
{
    Task NotifyDocumentRemarkAsync(int loanApplicationId, int documentChecklistId, string remarkType, string content, CancellationToken ct = default);
}