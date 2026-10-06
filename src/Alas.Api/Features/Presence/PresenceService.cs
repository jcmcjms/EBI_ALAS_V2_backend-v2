using System.Text.Json;
using StackExchange.Redis;

namespace Alas.Api.Features.Presence;

public sealed class PresenceService : IPresenceService
{
    private readonly IConnectionMultiplexer _redis;
    private const string SessionKeyPrefix = "presence:session:";
    private const string UserSessionsKey = "presence:user_sessions";
    private const string ConnectionToUserKeyPrefix = "presence:conn:";
    private const int MaxConnectionsPerUser = 10;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public PresenceService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private IDatabase Db => _redis.GetDatabase();

    private static string GetString(RedisValue value) => value.IsNullOrEmpty ? string.Empty : value.ToString();
    private static T? Deserialize<T>(RedisValue value) where T : class =>
        value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);

    public async ValueTask<bool> SetOnlineAsync(PresenceUserInfo user, string connectionId, CancellationToken ct = default)
    {
        var currentCount = await ConnectionCountAsync(user.UserId, ct);
        if (currentCount >= MaxConnectionsPerUser)
            return false;

        var sessionKey = $"{SessionKeyPrefix}{user.UserId}";
        var userJson = JsonSerializer.Serialize(user, JsonOptions);
        
        // Store/update user info
        await Db.StringSetAsync(sessionKey, userJson);
        
        // Add connection to user's session set
        await Db.SetAddAsync($"presence:connections:{user.UserId}", connectionId);
        
        // Maintain reverse index: connectionId -> userId (for O(1) ForgetConnection)
        await Db.StringSetAsync($"{ConnectionToUserKeyPrefix}{connectionId}", user.UserId.ToString());
        
        // Track user in global online users set
        await Db.SetAddAsync(UserSessionsKey, user.UserId.ToString());
        
        // Check if this was a new user (was offline)
        var wasOnline = currentCount > 0;
        return !wasOnline;
    }

    public async ValueTask<PresenceUserInfo?> SetOfflineAsync(int userId, string connectionId, CancellationToken ct = default)
    {
        var sessionKey = $"{SessionKeyPrefix}{userId}";
        var connectionsKey = $"presence:connections:{userId}";
        var connUserKey = $"{ConnectionToUserKeyPrefix}{connectionId}";

        // Remove connection from user's set
        var removed = await Db.SetRemoveAsync(connectionsKey, connectionId);
        if (!removed) return null;

        // Remove reverse index
        await Db.KeyDeleteAsync(connUserKey);

        // Check if user has any remaining connections
        var remainingCount = await Db.SetLengthAsync(connectionsKey);
        if (remainingCount > 0)
        {
            // User still has other connections, just return null (not fully offline)
            return null;
        }

        // User is fully offline - clean up
        var userJson = await Db.StringGetAsync(sessionKey);
        await Db.KeyDeleteAsync(sessionKey);
        await Db.KeyDeleteAsync(connectionsKey);
        await Db.SetRemoveAsync(UserSessionsKey, userId.ToString());

        if (string.IsNullOrEmpty(userJson)) return null;
        
        return Deserialize<PresenceUserInfo>(userJson);
    }

    public async ValueTask<bool> IsOnlineAsync(int userId, CancellationToken ct = default)
    {
        var connectionsKey = $"presence:connections:{userId}";
        var count = await Db.SetLengthAsync(connectionsKey);
        return count > 0;
    }

    public async ValueTask<int> ConnectionCountAsync(int userId, CancellationToken ct = default)
    {
        var connectionsKey = $"presence:connections:{userId}";
        return (int)await Db.SetLengthAsync(connectionsKey);
    }

    public async ValueTask<IReadOnlyList<PresenceEntry>> GetOnlineUsersAsync(CancellationToken ct = default)
    {
        var userIds = await Db.SetMembersAsync(UserSessionsKey);
        var result = new List<PresenceEntry>();

        foreach (var userIdValue in userIds)
        {
            if (!int.TryParse(userIdValue.ToString(), out var userId)) continue;
            
            var sessionKey = $"{SessionKeyPrefix}{userId}";
            var userJson = await Db.StringGetAsync(sessionKey);
            if (userJson.IsNullOrEmpty) continue;

            var user = Deserialize<PresenceUserInfo>(userJson);
            var count = await ConnectionCountAsync(userId, ct);
            
            if (user is not null)
                result.Add(new PresenceEntry(user, count));
        }

        return result.OrderBy(e => e.User.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public async ValueTask<int?> ForgetConnectionAsync(string connectionId, CancellationToken ct = default)
    {
        var connUserKey = $"{ConnectionToUserKeyPrefix}{connectionId}";
        var userIdStr = GetString(await Db.StringGetAsync(connUserKey));
        
        if (!int.TryParse(userIdStr, out var userId)) return null;

        return await SetOfflineAsync(userId, connectionId, ct) != null ? userId : null;
    }

}