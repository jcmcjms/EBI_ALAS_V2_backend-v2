namespace Alas.Api.Features.Presence;

public sealed record PresenceSnapshotEvent(IReadOnlyList<PresenceEntry> Users);

public sealed record PresenceChangedEvent(PresenceUserInfo User, bool Online, int Connections);

public sealed record EntityViewersChangedEvent(string EntityType, int EntityId, IReadOnlyList<EntityViewer> Viewers);

public sealed record WatchEntityRequest(string EntityType, int EntityId);

public sealed record UnwatchEntityRequest(string EntityType, int EntityId);

public sealed record PresenceFlagsRequest(IReadOnlyList<int> UserIds);

public sealed record PresenceFlagResponse(int UserId, bool Online, int? Connections);

public sealed record PresenceFlagsResponse(IReadOnlyList<PresenceFlagResponse> Flags);