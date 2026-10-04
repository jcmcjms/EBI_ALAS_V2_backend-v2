using EBI.ALAS.Api.Features.WebLoans;

namespace EBI.ALAS.Api.Features.WebLoans;

public interface IWebLoanRepository
{
    Task<CisInfo?> GetCisAsync(string cisNo, CancellationToken ct = default);
    Task<CisInfoMiscData?> GetCisMiscDataAsync(string cisNo, CancellationToken ct = default);
    Task<IReadOnlyList<LoanAcctInfo>> GetAccountsAsync(string cisNo, CancellationToken ct = default);
    Task<LoanAcctInfo?> GetAccountAsync(string cisNo, string accountNo, CancellationToken ct = default);
    Task<IReadOnlyList<LoanData>> GetLoanDataAsync(string accountNo, CancellationToken ct = default);
    Task<IReadOnlyList<OutstandingLoanRow>> GetOutstandingLoansAsync(string cisNo, CancellationToken ct = default);
}

public interface IWebLoanService
{
    Task<CisSearchResult> SearchAsync(string cisNo, CancellationToken ct = default);
    Task<CisInfo?> GetCisAsync(string cisNo, CancellationToken ct = default);
    Task<LoanAcctInfo?> GetAccountAsync(string cisNo, string accountNo, CancellationToken ct = default);
}

public sealed record CisSearchResult(string CisNo, string Name, IReadOnlyList<string> Accounts, string? MisGroup);