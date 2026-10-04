namespace EBI.ALAS.Api.Features.Loans.Computation;

public sealed record LoanProductComputationConfig(
    string ProductCode,
    string ProductName,
    decimal InterestRate,
    int TermDays,
    decimal MinAmount,
    decimal MaxAmount)
{
    public static LoanProductComputationConfig FromEntity(LoanProduct product, decimal interestRate, int termDays) =>
        new(product.ProductCode, product.ProductName, interestRate, termDays, product.MinAmount, product.MaxAmount);
}

public sealed record LoanFees(
    decimal ApplicationCharge,
    decimal DocStamp,
    decimal NotarialFee,
    decimal Insurance,
    decimal AdvanceInterest);

public sealed record LoanComputationInput(
    decimal ProposedAmount,
    LoanProductComputationConfig Product,
    LoanFees Fees,
    decimal NetTakeHomePay,
    decimal MinimumNthp,
    IReadOnlyList<decimal> OutstandingPrincipalBalances,
    IReadOnlyList<ObligationRow> Reloans,
    IReadOnlyList<ObligationRow> BuyOuts,
    IReadOnlyList<decimal> IncomingDeductions);

public sealed record ObligationRow(decimal Amortization, decimal OutstandingBalance);

public sealed record LoanComputationResult(
    decimal TotalDeductions,
    decimal DeductionRate,
    decimal GrossProceeds,
    decimal NetProceedsOnDS,
    decimal NetProceedsToClient,
    decimal TotalExposure,
    decimal? MonthlyAmortization,
    decimal NetPayAfterDeduction,
    decimal GrossDisposableIncome,
    decimal CapacityDeductions,
    decimal NetDisposableIncome,
    decimal MaximumLoanableAmount,
    bool AmortizationExceedsDisposable,
    bool NthpBelowMinimum);

public interface ILoanComputationService
{
    LoanFees ComputeExpectedFees(LoanProductComputationConfig config, decimal proposedAmount);
    LoanComputationResult ComputeLoanMetrics(LoanComputationInput input);
}

public sealed class LoanComputationService : ILoanComputationService
{
    public LoanFees ComputeExpectedFees(LoanProductComputationConfig config, decimal proposedAmount)
    {
        return new LoanFees(0, 0, 0, 0, 0);
    }

    public LoanComputationResult ComputeLoanMetrics(LoanComputationInput input)
    {
        return new LoanComputationResult(0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0, false, false);
    }
}