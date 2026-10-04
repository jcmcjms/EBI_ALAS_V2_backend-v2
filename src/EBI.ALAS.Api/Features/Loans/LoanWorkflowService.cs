using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Loans;

public interface ILoanWorkflowService
{
    string InitialStatus { get; }
    bool RequireRecommendation { get; }
    bool IsValidTransition(string fromStatus, string toStatus, string role);
    IEnumerable<string> GetValidTransitions(string fromStatus, string role);
}

public sealed class LoanWorkflowService(IWorkflowConfiguration config) : ILoanWorkflowService
{
    public string InitialStatus => config.InitialStatus;
    public bool RequireRecommendation => config.RequireRecommendation;

    public bool IsValidTransition(string fromStatus, string toStatus, string role) =>
        IsValidTransitionStatic(fromStatus, toStatus, role, RequireRecommendation);

    public static bool IsValidTransitionStatic(string fromStatus, string toStatus, string role, bool requireRecommendation)
    {
        var transitions = BuildTransitions(requireRecommendation);
        var key = (fromStatus, role);
        return transitions.TryGetValue(key, out var validTo) && validTo.Contains(toStatus);
    }

    public IEnumerable<string> GetValidTransitions(string fromStatus, string role)
    {
        var transitions = BuildTransitions(RequireRecommendation);
        var key = (fromStatus, role);
        return transitions.TryGetValue(key, out var validTo) ? validTo : [];
    }

    private static Dictionary<(string FromStatus, string Role), HashSet<string>> BuildTransitions(bool requireRecommendation)
    {
        var dict = new Dictionary<(string, string), HashSet<string>>();

        // Encoder transitions
        dict[("Draft", Roles.Encoder)] = ["ForRecommendation", "Cancelled"];
        if (!requireRecommendation)
        {
            dict[("Draft", Roles.Encoder)] = ["ForChecking", "Cancelled"];
        }
        dict[("ForRecommendation", Roles.Encoder)] = ["Cancelled"];
        dict[("ForChecking", Roles.Encoder)] = ["Cancelled"];
        dict[("ForApproval", Roles.Encoder)] = ["Cancelled"];
        dict[("ForRevision", Roles.Encoder)] = ["ForRecommendation"];
        if (!requireRecommendation)
        {
            dict[("ForRevision", Roles.Encoder)] = ["ForChecking"];
        }

        // Recommender transitions
        if (requireRecommendation)
        {
            dict[("ForRecommendation", Roles.Recommender)] = ["ForChecking", "ForRevision", "Cancelled"];
        }

        // Evaluator transitions
        dict[("ForChecking", Roles.Evaluator)] = ["ForApproval", "ForRevision", "Cancelled"];

        // Approver transitions
        dict[("ForApproval", Roles.Approver)] = ["Approved", "Rejected", "ForRevision", "Cancelled"];

        // Admin transitions (can do anything valid in state machine)
        var allStatuses = new[] { "Draft", "ForRecommendation", "ForChecking", "ForApproval", "ForRevision", "Approved", "Rejected", "Cancelled", "ForDisbursement", "Disbursed", "OnGoing" };
        foreach (var from in allStatuses)
        {
            var validTo = GetStateMachineTransitions(from);
            if (validTo.Count > 0)
            {
                dict[(from, Roles.Admin)] = validTo;
            }
        }

        return dict;
    }

    private static HashSet<string> GetStateMachineTransitions(string from)
    {
        return from switch
        {
            "Draft" => ["ForRecommendation", "ForChecking", "Cancelled"],
            "ForRecommendation" => ["ForChecking", "ForRevision", "Cancelled"],
            "ForChecking" => ["ForApproval", "ForRevision", "Cancelled"],
            "ForApproval" => ["Approved", "Rejected", "ForRevision", "Cancelled"],
            "ForRevision" => ["ForRecommendation", "ForChecking"],
            "Approved" => ["ForDisbursement"],
            "ForDisbursement" => ["Disbursed"],
            "Disbursed" => ["OnGoing"],
            _ => []
        };
    }
}