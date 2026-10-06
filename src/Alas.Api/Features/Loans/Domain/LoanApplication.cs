namespace Alas.Api.Features.Loans.Domain;

public sealed class LoanApplication
{
    public int Id { get; init; }
    public string LamId { get; set; } = string.Empty;
    public string ApplicationGroupNo { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public int? CreationTypeCode { get; set; }
    public string? CreationTypeLabel { get; set; }
    public string? CisId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public string? Address { get; set; }
    public string? Agency { get; set; }
    public string? Position { get; set; }
    public string? EmployeeId { get; set; }
    public decimal? NetTakeHomePay { get; set; }
    public string? LengthOfService { get; set; }
    public string? Region { get; set; }
    public string LoanNo { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public decimal ProposedAmount { get; set; }
    public int TermDays { get; set; }
    public decimal InterestRate { get; set; }
    public int? PolicyTermMonths { get; set; }
    public decimal NotarialFee { get; set; }
    public decimal DocStamps { get; set; }
    public decimal Insurance { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossProceeds { get; set; }
    public decimal NetProceedsToClient { get; set; }
    public decimal TotalExposure { get; set; }
    public decimal? MonthlyAmortization { get; set; }
    public bool HasDeviations { get; set; }
    public string? Remarks { get; set; }
    public string? AoRecommendation { get; set; }
    public string Status { get; set; } = LoanStatus.Draft;
    public DateTimeOffset ApplicationDate { get; init; }
    public DateTimeOffset LastActionDate { get; set; }
    public string LoanType { get; set; } = "New";
    public int CreatedById { get; init; }
    public int? AssignedApproverId { get; set; }
    public DateTimeOffset? AssignedAt { get; set; }
    public DateTimeOffset? DocumentsCompleteAt { get; set; }
}

public static class LoanStatus
{
    public const string Draft = "Draft";
    public const string ForRecommendation = "ForRecommendation";
    public const string ForChecking = "ForChecking";
    public const string ForApproval = "ForApproval";
    public const string ForRevision = "ForRevision";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Draft, ForRecommendation, ForChecking, ForApproval, ForRevision, Approved, Rejected, Cancelled
    };

    public static bool IsValid(string status) => All.Contains(status);
}