namespace Alas.Api.Features.WebLoans;

public interface IWebLoanService
{
    Task<CisSearchResponse?> SearchByCisAsync(string cisNo, string? branchCode, CancellationToken ct = default);
    Task<OutstandingLoansResponse?> GetOutstandingLoansAsync(string cisNo, string accountId, int pageSize = 50, int pageNumber = 1, CancellationToken ct = default);
    Task<PendingLoanResponse?> GetPendingLoanAsync(string cisNo, string accountId, CancellationToken ct = default);
    Task<IReadOnlyList<LoanProductDto>> GetActiveLoanProductsAsync(CancellationToken ct = default);
}