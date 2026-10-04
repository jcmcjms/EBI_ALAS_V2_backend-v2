namespace EBI.ALAS.Api.Features.Presence;

public sealed class EntityWatchService : IEntityWatchService
{
    public Task<IReadOnlyList<int>> GetViewersAsync(string entityType, int entityId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<int>>([]);

    public Task UserStartedViewingAsync(int userId, string entityType, int entityId) => Task.CompletedTask;

    public Task UserStoppedViewingAsync(int userId, string entityType, int entityId) => Task.CompletedTask;
}