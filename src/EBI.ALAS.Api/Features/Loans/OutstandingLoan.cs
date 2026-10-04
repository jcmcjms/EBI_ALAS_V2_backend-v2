namespace EBI.ALAS.Api.Features.Loans;

public sealed class OutstandingLoan
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string Pn { get; set; } = string.Empty;
    public decimal PrincipalBalance { get; set; }
    public decimal Amortization { get; set; }
    public decimal OutstandingBalance { get; set; }
    public DateOnly? DateGranted { get; set; }
    public DateOnly? DateMaturity { get; set; }
    public string? Status { get; set; }
    public string? ProductWithDescription { get; set; }
}