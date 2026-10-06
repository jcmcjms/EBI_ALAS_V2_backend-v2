namespace Alas.Api.Features.Notifications;

public interface IRealtimeNotificationService
{
    Task NotifyUserAsync(int userId, string title, string description, string? link, CancellationToken ct = default);
    Task NotifyBranchAsync(string branchCode, string title, string description, string? link, CancellationToken ct = default);
    Task NotifyAllAsync(string title, string description, string? link, CancellationToken ct = default);
}