namespace EBI.ALAS.Api.Features.Loans;

public sealed class EbiReloan
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string Pn { get; set; } = string.Empty;
    public string? Name { get; set; }
    public decimal ExistingDeduction { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal PayToClose { get; set; }
}