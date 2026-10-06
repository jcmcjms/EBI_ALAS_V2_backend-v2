namespace Alas.Api.Features.Presence;

public interface IPresenceService
{
    ValueTask<bool> SetOnlineAsync(PresenceUserInfo user, string connectionId, CancellationToken ct = default);
    ValueTask<PresenceUserInfo?> SetOfflineAsync(int userId, string connectionId, CancellationToken ct = default);
    ValueTask<bool> IsOnlineAsync(int userId, CancellationToken ct = default);
    ValueTask<int> ConnectionCountAsync(int userId, CancellationToken ct = default);
    ValueTask<IReadOnlyList<PresenceEntry>> GetOnlineUsersAsync(CancellationToken ct = default);
    ValueTask<int?> ForgetConnectionAsync(string connectionId, CancellationToken ct = default);
}