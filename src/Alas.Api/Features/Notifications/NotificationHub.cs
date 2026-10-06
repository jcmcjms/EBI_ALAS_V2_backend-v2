using Alas.Api.Features.Presence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Alas.Api.Features.Notifications;

[Authorize]
public sealed class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
    private readonly IPresenceService _presence;
    private readonly IEntityWatchService _watch;
    private const int MaxConnectionsPerUser = 10;

    public NotificationHub(ILogger<NotificationHub> logger, IPresenceService presence, IEntityWatchService watch)
    {
        _logger = logger;
        _presence = presence;
        _watch = watch;
    }

    public override async Task OnConnectedAsync()
    {
        var user = ReadUserFromClaims();
        if (user is null)
        {
            Context.Abort();
            return;
        }

        // Connection limit enforcement
        var currentCount = await _presence.ConnectionCountAsync(user.UserId);
        if (currentCount >= MaxConnectionsPerUser)
        {
            Context.Abort();
            return;
        }

        if (!string.IsNullOrEmpty(user.BranchCode))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Branch_{user.BranchCode}");

        await Groups.AddToGroupAsync(Context.ConnectionId, "All_Users");

        var becameOnline = await _presence.SetOnlineAsync(user, Context.ConnectionId);
        
        // Send snapshot to caller
        var onlineUsers = await _presence.GetOnlineUsersAsync();
        await Clients.Caller.SendAsync("PresenceSnapshot", new PresenceSnapshotEvent(onlineUsers));

        // Broadcast presence change if user came online
        if (becameOnline)
        {
            var connCount = await _presence.ConnectionCountAsync(user.UserId);
            await Clients.Group("All_Users").SendAsync("PresenceChanged", 
                new PresenceChangedEvent(user, true, connCount));
        }

        _logger.LogInformation("SignalR connection {ConnectionId} connected for user {UserId}", Context.ConnectionId, user.UserId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Handle entity watch cleanup
        foreach (var key in await _watch.DropConnectionAsync(Context.ConnectionId))
        {
            var viewers = await _watch.GetViewersAsync(key);
            await Clients.Group(key.Group).SendAsync("EntityViewersChanged",
                new EntityViewersChangedEvent(key.EntityType, key.EntityId, viewers));
        }

        var userId = ReadUserIdFromClaims();
        if (userId is { } id)
        {
            var removedUser = await _presence.SetOfflineAsync(id, Context.ConnectionId);
            if (removedUser is not null)
            {
                await Clients.Group("All_Users").SendAsync("PresenceChanged",
                    new PresenceChangedEvent(removedUser, false, 0));
            }
        }

        _logger.LogInformation("SignalR connection {ConnectionId} disconnected for user {UserId}. Reason: {Reason}", 
            Context.ConnectionId, userId, exception?.Message ?? "Normal closure");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task WatchEntity(WatchEntityRequest request)
    {
        var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LoanApplication",
            "DocumentRemark",
            "LoanDeviation",
            "ChecklistDocument",
        };

        if (!allowedTypes.Contains(request.EntityType)) return;

        var user = ReadUserFromClaims();
        if (user is null) return;

        var key = new EntityWatchKey(request.EntityType, request.EntityId);
        await Groups.AddToGroupAsync(Context.ConnectionId, key.Group);

        var viewers = await _watch.WatchAsync(Context.ConnectionId, user, key);
        await Clients.Group(key.Group).SendAsync("EntityViewersChanged",
            new EntityViewersChangedEvent(key.EntityType, key.EntityId, viewers));
    }

    public async Task UnwatchEntity(UnwatchEntityRequest request)
    {
        var key = new EntityWatchKey(request.EntityType, request.EntityId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, key.Group);

        var viewers = await _watch.UnwatchAsync(Context.ConnectionId, key);
        await Clients.Group(key.Group).SendAsync("EntityViewersChanged",
            new EntityViewersChangedEvent(key.EntityType, key.EntityId, viewers));
    }

    private PresenceUserInfo? ReadUserFromClaims()
    {
        var principal = Context.User;
        if (principal is null) return null;

        var userIdClaim = principal.FindFirst("userId");
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            return null;

        var firstName = principal.FindFirst("firstName")?.Value ?? string.Empty;
        var lastName = principal.FindFirst("lastName")?.Value ?? string.Empty;
        var name = $"{firstName} {lastName}".Trim();
        if (string.IsNullOrEmpty(name))
            name = principal.FindFirst("username")?.Value ?? string.Empty;

        return new PresenceUserInfo(
            UserId: userId,
            Name: name,
            Role: principal.FindFirst("role")?.Value ?? string.Empty,
            BranchCode: principal.FindFirst("branchCode")?.Value ?? string.Empty,
            JobTitle: principal.FindFirst("jobTitle")?.Value);
    }

    private int? ReadUserIdFromClaims()
    {
        var userIdClaim = Context.User?.FindFirst("userId");
        return userIdClaim is not null && int.TryParse(userIdClaim.Value, out var id) ? id : null;
    }
}