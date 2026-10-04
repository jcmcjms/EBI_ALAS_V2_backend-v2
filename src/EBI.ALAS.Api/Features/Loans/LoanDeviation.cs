namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanDeviation
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string ReasonText { get; set; } = string.Empty;
    public string EncoderJustification { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsFeeOverride { get; set; }
    public const string FeeOverrideReason = "FEE_OVERRIDE";
}