namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanProductChecklist
{
    public int Id { get; init; }
    public string ProductCode { get; set; } = string.Empty;
    public string DocumentCode { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public int SortOrder { get; set; }
}