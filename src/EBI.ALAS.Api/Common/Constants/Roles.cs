namespace EBI.ALAS.Api.Common.Constants;

public static class Roles
{
    public const string Encoder = "Encoder";
    public const string Recommender = "Recommender";
    public const string Evaluator = "Evaluator";
    public const string Approver = "Approver";
    public const string Admin = "Admin";

    public static readonly IReadOnlyList<string> All =
    [
        Encoder,
        Recommender,
        Evaluator,
        Approver,
        Admin
    ];

    public static readonly IReadOnlyList<string> WorkflowRoles =
    [
        Encoder,
        Recommender,
        Evaluator,
        Approver
    ];

    public static bool IsWorkflowRole(string role) =>
        WorkflowRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
}