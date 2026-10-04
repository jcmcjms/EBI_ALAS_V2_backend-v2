using System.Text.Json;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanSubmissionIdempotency
{
    public int Id { get; init; }
    public Guid IdempotencyKey { get; set; }
    public int UserId { get; set; }
    public string ResponseJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}