namespace Alas.Api.Features.Loans.Domain;

public sealed class LoanProduct
{
    public int Id { get; init; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int MinTermDays { get; set; }
    public int MaxTermDays { get; set; }
    public decimal NotarialFee { get; set; }
    public decimal DocStampFee { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal AdvanceInterestRate { get; set; }
    public decimal ApplicationChargeRate { get; set; }
    public string AmortizationMode { get; set; } = "DIM";
    public bool ChargeAdvanceInterest { get; set; }
    public bool IsRetired { get; set; }
    public DateTimeOffset LastSyncedAt { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public int? UpdatedById { get; set; }
}