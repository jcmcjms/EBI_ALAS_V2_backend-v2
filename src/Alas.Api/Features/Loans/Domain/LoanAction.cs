namespace Alas.Api.Features.Loans.Domain;

public sealed class LoanAction
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? FromStatus { get; set; }
    public string? ToStatus { get; set; }
    public string? Comments { get; set; }
    public int ActionByUserId { get; set; }
    public DateTimeOffset ActionDate { get; set; }
    public string ActionByRole { get; set; } = string.Empty;
    public LoanApplication LoanApplication { get; set; } = null!;
}