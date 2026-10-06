namespace Alas.Api.Features.Presence;

public interface IEntityWatchService
{
    ValueTask<IReadOnlyList<EntityViewer>> WatchAsync(string connectionId, PresenceUserInfo user, EntityWatchKey key, CancellationToken ct = default);
    ValueTask<IReadOnlyList<EntityViewer>> UnwatchAsync(string connectionId, EntityWatchKey key, CancellationToken ct = default);
    ValueTask<IReadOnlyList<EntityWatchKey>> DropConnectionAsync(string connectionId, CancellationToken ct = default);
    ValueTask<IReadOnlyList<EntityViewer>> GetViewersAsync(EntityWatchKey key, CancellationToken ct = default);
}