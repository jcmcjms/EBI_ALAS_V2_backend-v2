using System.Collections.Concurrent;

namespace EBI.ALAS.Api.Features.Presence;

public sealed class PresenceService : IPresenceService, IEntityWatchService
{
    private readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();
    private readonly ConcurrentDictionary<string, (int UserId, DateTime ConnectedAt)> _connectionInfo = new();
    private readonly ConcurrentDictionary<(string EntityType, int EntityId), HashSet<int>> _entityViewers = new();

    public Task<IReadOnlyList<OnlineUserDto>> GetOnlineUsersAsync(int currentUserId, string? branchCode, bool isAdmin, CancellationToken ct = default)
    {
        var online = _userConnections.Keys.Select(id => new OnlineUserDto(id, "User", "Role", "Branch", DateTime.UtcNow)).ToList();
        return Task.FromResult<IReadOnlyList<OnlineUserDto>>(online);
    }

    public Task<IReadOnlyList<PresenceDto>> GetPresenceAsync(IEnumerable<int> userIds, CancellationToken ct = default)
    {
        var result = userIds.Select(id => new PresenceDto(id, _userConnections.ContainsKey(id))).ToList();
        return Task.FromResult<IReadOnlyList<PresenceDto>>(result);
    }

    public Task UserConnectedAsync(int userId, string connectionId)
    {
        _userConnections.AddOrUpdate(userId,
            _ => new HashSet<string> { connectionId },
            (_, set) => { set.Add(connectionId); return set; });
        _connectionInfo[connectionId] = (userId, DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public Task UserDisconnectedAsync(int userId, string connectionId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            connections.Remove(connectionId);
            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId, out _);
            }
        }
        _connectionInfo.TryRemove(connectionId, out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<int>> GetViewersAsync(string entityType, int entityId, CancellationToken ct = default)
    {
        var key = (entityType, entityId);
        if (_entityViewers.TryGetValue(key, out var viewers))
        {
            return Task.FromResult<IReadOnlyList<int>>(viewers.ToList());
        }
        return Task.FromResult<IReadOnlyList<int>>([]);
    }

    public Task UserStartedViewingAsync(int userId, string entityType, int entityId)
    {
        var key = (entityType, entityId);
        _entityViewers.AddOrUpdate(key,
            _ => new HashSet<int> { userId },
            (_, set) => { set.Add(userId); return set; });
        return Task.CompletedTask;
    }

    public Task UserStoppedViewingAsync(int userId, string entityType, int entityId)
    {
        var key = (entityType, entityId);
        if (_entityViewers.TryGetValue(key, out var viewers))
        {
            viewers.Remove(userId);
            if (viewers.Count == 0)
            {
                _entityViewers.TryRemove(key, out _);
            }
        }
        return Task.CompletedTask;
    }
}