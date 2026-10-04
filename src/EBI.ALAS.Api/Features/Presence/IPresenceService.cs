using System.Security.Claims;

namespace EBI.ALAS.Api.Features.Presence;

public interface IPresenceService
{
    Task<IReadOnlyList<OnlineUserDto>> GetOnlineUsersAsync(int currentUserId, string? branchCode, bool isAdmin, CancellationToken ct = default);
    Task<IReadOnlyList<PresenceDto>> GetPresenceAsync(IEnumerable<int> userIds, CancellationToken ct = default);
    Task UserConnectedAsync(int userId, string connectionId);
    Task UserDisconnectedAsync(int userId, string connectionId);
}

public interface IEntityWatchService
{
    Task<IReadOnlyList<int>> GetViewersAsync(string entityType, int entityId, CancellationToken ct = default);
    Task UserStartedViewingAsync(int userId, string entityType, int entityId);
    Task UserStoppedViewingAsync(int userId, string entityType, int entityId);
}

public sealed record OnlineUserDto(int Id, string Name, string Role, string BranchCode, DateTime LastSeen);
public sealed record PresenceDto(int UserId, bool IsOnline);