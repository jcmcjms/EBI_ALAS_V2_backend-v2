using Microsoft.AspNetCore.SignalR;

namespace Alas.Api.Features.Notifications;

public sealed class RealtimeNotificationService : IRealtimeNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly TimeProvider _timeProvider;

    public RealtimeNotificationService(IHubContext<NotificationHub> hubContext, TimeProvider timeProvider)
    {
        _hubContext = hubContext;
        _timeProvider = timeProvider;
    }

    public async Task NotifyUserAsync(int userId, string title, string description, string? link, CancellationToken ct = default)
    {
        await _hubContext.Clients
            .Group($"User_{userId}")
            .SendAsync("ReceiveNotification", new NotificationPush(
                Title: title,
                Description: description,
                Link: link,
                Timestamp: _timeProvider.GetUtcNow()), ct);
    }

    public async Task NotifyBranchAsync(string branchCode, string title, string description, string? link, CancellationToken ct = default)
    {
        await _hubContext.Clients
            .Group($"Branch_{branchCode}")
            .SendAsync("ReceiveNotification", new NotificationPush(
                Title: title,
                Description: description,
                Link: link,
                Timestamp: _timeProvider.GetUtcNow()), ct);
    }

    public async Task NotifyAllAsync(string title, string description, string? link, CancellationToken ct = default)
    {
        await _hubContext.Clients
            .All
            .SendAsync("ReceiveNotification", new NotificationPush(
                Title: title,
                Description: description,
                Link: link,
                Timestamp: _timeProvider.GetUtcNow()), ct);
    }
}