namespace EBI.ALAS.Api.Common.Constants;

public static class RoleQueues
{
    public static readonly IReadOnlyDictionary<string, string> RoleToQueuePrefix = new Dictionary<string, string>
    {
        [Roles.Recommender] = "REC",
        [Roles.Evaluator] = "EVA",
        [Roles.Approver] = "APP"
    };

    public static string GetQueuePrefix(string role) =>
        RoleToQueuePrefix.TryGetValue(role, out var prefix) ? prefix : string.Empty;

    public static string GetQueuePartition(string role, string branchCode, int? tier = null)
    {
        var prefix = GetQueuePrefix(role);
        if (string.IsNullOrEmpty(prefix))
        {
            return string.Empty;
        }

        return tier.HasValue && role == Roles.Approver
            ? $"{prefix}:{branchCode}:{tier}"
            : $"{prefix}:{branchCode}";
    }
}