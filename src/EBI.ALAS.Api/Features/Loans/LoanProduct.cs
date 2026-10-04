namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanProduct
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal InterestRate { get; set; }
    public int MinTermDays { get; set; }
    public int MaxTermDays { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}