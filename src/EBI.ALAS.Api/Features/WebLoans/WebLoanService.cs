using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Infrastructure.Data;
using EBI.ALAS.Api.Features.WebLoans;

namespace EBI.ALAS.Api.Features.WebLoans;

public sealed class WebLoanRepository(WebLoanDbContext db) : IWebLoanRepository
{
    public Task<CisInfo?> GetCisAsync(string cisNo, CancellationToken ct = default) =>
        db.CisInfo.AsNoTracking().FirstOrDefaultAsync(c => c.CisNo == cisNo, ct);

    public Task<CisInfoMiscData?> GetCisMiscDataAsync(string cisNo, CancellationToken ct = default) =>
        db.CisInfoMiscData.AsNoTracking().FirstOrDefaultAsync(c => c.CisNo == cisNo, ct);

    public Task<IReadOnlyList<LoanAcctInfo>> GetAccountsAsync(string cisNo, CancellationToken ct = default) =>
        db.LoanAcctInfos.AsNoTracking().Where(a => a.CisNo == cisNo).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<LoanAcctInfo>)t.Result);

    public Task<LoanAcctInfo?> GetAccountAsync(string cisNo, string accountNo, CancellationToken ct = default) =>
        db.LoanAcctInfos.AsNoTracking().FirstOrDefaultAsync(a => a.CisNo == cisNo && a.AcctNo == accountNo, ct);

    public Task<IReadOnlyList<LoanData>> GetLoanDataAsync(string accountNo, CancellationToken ct = default) =>
        db.LoanData.AsNoTracking().Where(l => l.AcctNo == accountNo).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<LoanData>)t.Result);

    public Task<IReadOnlyList<OutstandingLoanRow>> GetOutstandingLoansAsync(string cisNo, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<OutstandingLoanRow>>([]);
}

public sealed class WebLoanService(IWebLoanRepository webLoanRepository) : IWebLoanService
{
    public async Task<CisSearchResult> SearchAsync(string cisNo, CancellationToken ct = default)
    {
        var cis = await webLoanRepository.GetCisAsync(cisNo, ct);
        var misData = await webLoanRepository.GetCisMiscDataAsync(cisNo, ct);
        var accounts = await webLoanRepository.GetAccountsAsync(cisNo, ct);

        if (cis is null)
        {
            return new CisSearchResult(cisNo, "", [], null);
        }

        var name = $"{cis.FirstName} {cis.LastName}".Trim();
        var accountNos = accounts.Select(a => a.AcctNo).ToList();

        return new CisSearchResult(cisNo, name, accountNos, misData?.MisAgency);
    }

    public Task<CisInfo?> GetCisAsync(string cisNo, CancellationToken ct = default) =>
        webLoanRepository.GetCisAsync(cisNo, ct);

    public Task<LoanAcctInfo?> GetAccountAsync(string cisNo, string accountNo, CancellationToken ct = default) =>
        webLoanRepository.GetAccountAsync(cisNo, accountNo, ct);
}