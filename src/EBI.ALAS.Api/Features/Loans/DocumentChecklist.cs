namespace EBI.ALAS.Api.Features.Loans;

public sealed class DocumentChecklist
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string DocumentCode { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Submitted, Verified, Rejected
    public string? Remarks { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? SubmittedById { get; set; }
    public bool IsRequired { get; set; } = true;
    public int SortOrder { get; set; }
}