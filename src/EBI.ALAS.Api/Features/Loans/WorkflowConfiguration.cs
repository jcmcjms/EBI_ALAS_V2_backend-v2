using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class WorkflowConfiguration : IWorkflowConfiguration
{
    public int Id { get; init; }
    public bool RequireRecommendation { get; set; }
    public int SlaForRecommendationHours { get; set; } = 4;
    public int SlaForCheckingHours { get; set; } = 8;
    public int SlaForApprovalHours { get; set; } = 8;
    public int SlaForRevisionHours { get; set; } = 24;
    public int SlaForDisbursementHours { get; set; } = 24;
    public decimal MinimumNthp { get; set; } = 5000;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public const string SectionName = "Workflow";

    // Computed properties for easy access
    public string InitialStatus => RequireRecommendation ? "ForRecommendation" : "ForChecking";
    public bool RequireRecommendationStep => RequireRecommendation;
    public bool IsValidTransition(string from, string to, string role) => LoanWorkflowService.IsValidTransitionStatic(from, to, role, RequireRecommendation);
}