namespace EBI.ALAS.Api.Common.Constants;

public sealed class WorkflowOptions
{
    public const string SectionName = "Workflow";
    public bool RequireRecommendation { get; set; }
    public int SlaForRecommendationHours { get; set; } = 4;
    public int SlaForCheckingHours { get; set; } = 8;
    public int SlaForApprovalHours { get; set; } = 8;
    public int SlaForRevisionHours { get; set; } = 24;
    public int SlaForDisbursementHours { get; set; } = 24;
    public decimal MinimumNthp { get; set; } = 5000;
}

public sealed class QueueOptions
{
    public const string SectionName = "Queue";
    public int MaxConcurrentAssignments { get; set; } = 5;
    public int AutoReleaseMinutes { get; set; } = 60;
    public int ReconciliationIntervalMinutes { get; set; } = 15;
}