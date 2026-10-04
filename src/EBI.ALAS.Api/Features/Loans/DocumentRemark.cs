namespace EBI.ALAS.Api.Features.Loans;

public sealed class DocumentRemark
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public int DocumentChecklistId { get; set; }
    public int UserId { get; set; }
    public string RemarkType { get; set; } = string.Empty; // Query, Instruction, Note
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int? ResolvedById { get; set; }
}