using System.Text.Json;
using StackExchange.Redis;

namespace Alas.Api.Features.Presence;

public sealed class EntityWatchService : IEntityWatchService
{
    private readonly IConnectionMultiplexer _redis;
    private const string GroupKeyPrefix = "presence:watch:";
    private const string ConnectionWatchesKeyPrefix = "presence:conn_watches:";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public EntityWatchService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private IDatabase Db => _redis.GetDatabase();

    private static string GetString(RedisValue value) => value.IsNullOrEmpty ? string.Empty : value.ToString();
    private static T? Deserialize<T>(RedisValue value) where T : class =>
        value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);

    private string GetGroupKey(EntityWatchKey key) => $"{GroupKeyPrefix}{key.EntityType}:{key.EntityId}";
    private string GetConnectionsKey(EntityWatchKey key) => $"{GetGroupKey(key)}:connections";
    private string GetWatchersKey(EntityWatchKey key) => $"{GetGroupKey(key)}:watchers";
    private string GetConnWatchesKey(string connectionId) => $"{ConnectionWatchesKeyPrefix}{connectionId}";

    public async ValueTask<IReadOnlyList<EntityViewer>> WatchAsync(string connectionId, PresenceUserInfo user, EntityWatchKey key, CancellationToken ct = default)
    {
        var connectionsKey = GetConnectionsKey(key);
        var watchersKey = GetWatchersKey(key);
        var connWatchesKey = GetConnWatchesKey(connectionId);

        // Add connection to entity's connection set
        await Db.SetAddAsync(connectionsKey, connectionId);

        // Store/update watcher info
        var watcherJson = JsonSerializer.Serialize(new EntityViewer(user.UserId, user.Name), JsonOptions);
        await Db.HashSetAsync(watchersKey, user.UserId.ToString(), watcherJson);

        // Track which entities this connection is watching
        await Db.SetAddAsync(connWatchesKey, key.ToString());

        return await GetViewersAsync(key, ct);
    }

    public async ValueTask<IReadOnlyList<EntityViewer>> UnwatchAsync(string connectionId, EntityWatchKey key, CancellationToken ct = default)
    {
        await RemoveMembershipAsync(connectionId, key, ct);
        return await GetViewersAsync(key, ct);
    }

    public async ValueTask<IReadOnlyList<EntityWatchKey>> DropConnectionAsync(string connectionId, CancellationToken ct = default)
    {
        var connWatchesKey = GetConnWatchesKey(connectionId);
        var entityKeys = await Db.SetMembersAsync(connWatchesKey);
        var changed = new List<EntityWatchKey>();

        foreach (var keyValue in entityKeys)
        {
            var keyStr = keyValue.ToString();
            if (EntityWatchKey.TryParse(keyStr, out var key) && key is not null)
            {
                await RemoveMembershipAsync(connectionId, key, ct);
                changed.Add(key);
            }
        }

        // Clean up connection's watch list
        await Db.KeyDeleteAsync(connWatchesKey);

        return changed;
    }

    public async ValueTask<IReadOnlyList<EntityViewer>> GetViewersAsync(EntityWatchKey key, CancellationToken ct = default)
    {
        var watchersKey = GetWatchersKey(key);
        var connectionsKey = GetConnectionsKey(key);

        var watcherEntries = await Db.HashGetAllAsync(watchersKey);
        var activeConnectionIds = await Db.SetMembersAsync(connectionsKey);
        var activeConnectionSet = new HashSet<string>(activeConnectionIds.Select(v => v.ToString()));

        var result = new List<EntityViewer>();

        foreach (var entry in watcherEntries)
        {
            var viewer = Deserialize<EntityViewer>(entry.Value);
            if (viewer is not null)
            {
                result.Add(viewer);
            }
        }

        return result.OrderBy(v => v.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private async ValueTask RemoveMembershipAsync(string connectionId, EntityWatchKey key, CancellationToken ct = default)
    {
        var connectionsKey = GetConnectionsKey(key);
        var watchersKey = GetWatchersKey(key);
        var connWatchesKey = GetConnWatchesKey(connectionId);

        // Remove connection from entity's connection set
        await Db.SetRemoveAsync(connectionsKey, connectionId);

        // Remove from connection's watch list
        await Db.SetRemoveAsync(connWatchesKey, key.ToString());

        // Check if any connections remain for this entity
        var remainingConnections = await Db.SetLengthAsync(connectionsKey);
        if (remainingConnections == 0)
        {
            // Clean up empty entity
            await Db.KeyDeleteAsync(connectionsKey);
            await Db.KeyDeleteAsync(watchersKey);
        }
    }

}