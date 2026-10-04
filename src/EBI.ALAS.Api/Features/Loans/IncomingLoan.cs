namespace EBI.ALAS.Api.Features.Loans;

public sealed class IncomingLoan
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string? Name { get; set; }
    public decimal Deductions { get; set; }
    public string? Remarks { get; set; }
}