namespace Alas.Api.Features.Presence;

public sealed record PresenceUserInfo(
    int UserId,
    string Name,
    string Role,
    string BranchCode,
    string? JobTitle);

public sealed record PresenceEntry(PresenceUserInfo User, int Connections);

public sealed record PresenceChange(PresenceUserInfo User, bool Online, int Connections);

public sealed record EntityViewer(int UserId, string Name);

public sealed record EntityWatchKey(string EntityType, int EntityId)
{
    public string Group => $"watch:{EntityType}:{EntityId}";
    public override string ToString() => $"{EntityType}:{EntityId}";

    public static bool TryParse(string s, out EntityWatchKey? key)
    {
        key = null;
        if (string.IsNullOrEmpty(s)) return false;
        var parts = s.Split(':', 2);
        if (parts.Length != 2) return false;
        if (!int.TryParse(parts[1], out var entityId)) return false;
        key = new EntityWatchKey(parts[0], entityId);
        return true;
    }
}