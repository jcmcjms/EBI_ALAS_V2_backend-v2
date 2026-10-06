using Alas.Api.Features.WebLoans.Domain;

namespace Alas.Api.Features.WebLoans;

public interface IWebLoanRepository
{
    Task<CisInfo?> GetCisInfoAsync(string cisNo, CancellationToken ct = default);
    Task<CisInfoMiscData?> GetAgencyTypeAsync(string cisNo, CancellationToken ct = default);
    Task<CheckListData?> GetLengthOfServiceAsync(string cisNo, CancellationToken ct = default);
    Task<MisGroup?> GetMisGroupByIdCodeAsync(string idCode, CancellationToken ct = default);
    Task<IReadOnlyList<LoanAcctInfo>> GetAccountsByCisAsync(string cisNo, string? branchCode = null, CancellationToken ct = default);
    Task<bool> AccountBelongsToCisAsync(string cisNo, string branchCode, string accountNo, CancellationToken ct = default);
    Task<IReadOnlyList<OutstandingLoanRow>> GetOutstandingLoansAsync(string branchCode, string accountNo, int pageSize = 50, int pageNumber = 1, CancellationToken ct = default);
    Task<IReadOnlyList<PendingLoanRow>> GetPendingLoansAsync(string branchCode, string accountNo, CancellationToken ct = default);
    Task<IReadOnlyList<LoanProductLookup>> GetActiveLoanProductsAsync(CancellationToken ct = default);
}