namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public sealed class UserBranchCoverage
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string CoverageType { get; set; } = string.Empty; // Branch, Area, Global
    public string? AreaCode { get; set; }
}