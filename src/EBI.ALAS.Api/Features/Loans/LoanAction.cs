using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanAction
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public int ActionById { get; set; }
    public string Action { get; set; } = string.Empty;
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime ActionDate { get; init; } = DateTime.UtcNow;

    public User? ActionByUser { get; set; }
}