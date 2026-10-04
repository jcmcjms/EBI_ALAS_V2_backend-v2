namespace EBI.ALAS.Api.Features.Loans;

public interface IWorkflowConfiguration
{
    bool RequireRecommendation { get; }
    int SlaForRecommendationHours { get; }
    int SlaForCheckingHours { get; }
    int SlaForApprovalHours { get; }
    int SlaForRevisionHours { get; }
    int SlaForDisbursementHours { get; }
    decimal MinimumNthp { get; }
    string InitialStatus { get; }
    bool IsValidTransition(string from, string to, string role);
}