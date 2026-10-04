namespace EBI.ALAS.Api.Features.Loans;

public sealed class DeviationRemark
{
    public int Id { get; init; }
    public int LoanDeviationId { get; set; }
    public int UserId { get; set; }
    public string Remark { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}