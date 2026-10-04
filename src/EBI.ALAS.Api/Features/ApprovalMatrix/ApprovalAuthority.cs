namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public enum DeviationSeverity
{
    None = 0,
    Minor = 1,
    Major = 2
}

public sealed class ApprovalAuthority
{
    public int Id { get; init; }
    public string LoanType { get; set; } = string.Empty; // New, Renewal
    public decimal MinExposure { get; set; }
    public decimal MaxExposure { get; set; }
    public DeviationSeverity DeviationSeverity { get; set; }
    public int Tier { get; set; }
    public int Priority { get; set; }
    public string ApproverRole { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public string? AreaCode { get; set; }
    public bool IsActive { get; set; } = true;
}