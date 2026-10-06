using Alas.Api.Features.WebLoans.Domain;
using Alas.Api.Infrastructure.Data.WebLoan;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.WebLoans;

public sealed class WebLoanRepository : IWebLoanRepository
{
    private readonly IDbContextFactory<WebLoanDbContext> _contextFactory;

    public WebLoanRepository(IDbContextFactory<WebLoanDbContext> contextFactory) =>
        _contextFactory = contextFactory;

    public async Task<CisInfo?> GetCisInfoAsync(string cisNo, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.CisInfos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CisNo == cisNo, ct);
    }

    public async Task<CisInfoMiscData?> GetAgencyTypeAsync(string cisNo, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.CisInfoMiscDatas
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CisNo == cisNo && m.IdCode == CisInfoMiscData.AgencyTypeIdCode, ct);
    }

    public async Task<CheckListData?> GetLengthOfServiceAsync(string cisNo, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.CheckListDatas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CisNo == cisNo && c.CheckListItem == CheckListData.LengthOfServiceItem, ct);
    }

    public async Task<MisGroup?> GetMisGroupByIdCodeAsync(string idCode, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.MisGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.IdCode == idCode, ct);
    }

    public async Task<IReadOnlyList<LoanAcctInfo>> GetAccountsByCisAsync(string cisNo, string? branchCode = null, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        var query = context.LoanAcctInfos
            .AsNoTracking()
            .Where(a => a.CisNo == cisNo);

        if (!string.IsNullOrEmpty(branchCode))
            query = query.Where(a => a.BranchCode == branchCode);

        return await query.OrderBy(a => a.AccountNo).ToListAsync(ct);
    }

    public async Task<bool> AccountBelongsToCisAsync(string cisNo, string branchCode, string accountNo, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.LoanAcctInfos
            .AsNoTracking()
            .AnyAsync(a => a.CisNo == cisNo && a.BranchCode == branchCode && a.AccountNo == accountNo, ct);
    }

    public async Task<IReadOnlyList<OutstandingLoanRow>> GetOutstandingLoansAsync(
        string branchCode, string accountNo, int pageSize = 50, int pageNumber = 1, CancellationToken ct = default)
    {
        FormattableString sql = $@"
            SELECT
                ld.loan_no AS LoanNo,
                ld.principal AS Principal,
                ld.principal_bal AS PrincipalBalance,
                CASE
                    WHEN ld.loan_product IN ('C35','C23') THEN ld.principal
                    ELSE ad.total_amort
                END AS ComputedAmortAmount,
                ld.date_granted AS DateGranted,
                ld.date_maturity AS DateMaturity,
                ISNULL(ld.loan_product, '') AS ProductCode,
                ld.loan_status AS StatusCode,
                ISNULL(ld.loan_product, '') + ' - ' + ISNULL(lp.description, '') AS ProductWithDescription
            FROM webloan.dbo.loan_data AS ld
            LEFT JOIN webloan.dbo.amort_data AS ad
                ON ld.loan_no = ad.loan_no
                AND ld.acct_no = ad.acct_no
                AND ld.bch = ad.bch
                AND ad.amort_no = 1
            LEFT JOIN webloan.dbo.loan_product AS lp
                ON ld.loan_product = lp.id_code
            WHERE ld.acct_no = {accountNo}
              AND ld.bch = {branchCode}
              AND ld.loan_status != 10
              AND ld.principal_bal > 0
            ORDER BY ld.date_granted DESC
            OFFSET {(pageNumber - 1) * pageSize} ROWS
            FETCH NEXT {pageSize} ROWS ONLY";

        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.OutstandingLoanRows
            .FromSqlInterpolated(sql)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PendingLoanRow>> GetPendingLoansAsync(
        string branchCode, string accountNo, CancellationToken ct = default)
    {
        FormattableString sql = $@"
            SELECT
                pld.loan_no AS LoanNo,
                ld.principal AS Principal,
                ld.granted_rate AS GrantedRate,
                DATEDIFF(DAY, ld.date_granted, ld.date_maturity) AS TotalTermDays,
                ld.total_amortization AS TotalAmortization,
                ISNULL(ld.loan_product, '') + ' - ' + ISNULL(lp.description, '') AS ProductWithDescription,
                lp2.description AS LoanPurpose,
                ld.creation_type AS CreationType,
                CASE ld.creation_type
                    WHEN 0 THEN 'New Loan'
                    WHEN 1 THEN 'Reloan'
                    WHEN 2 THEN 'Restructured'
                    WHEN 6 THEN 'Additional Loan'
                    ELSE 'Unknown'
                END AS CreationTypeLabel,
                ld.c_doc_stamp AS CDocStamp,
                cld.description AS Nthp,
                cld.expiration AS NthpDate
            FROM webloan.dbo.pre_loan_data AS pld
            LEFT JOIN webloan.dbo.loan_data AS ld
                ON pld.loan_no = ld.loan_no
                AND pld.acct_no = ld.acct_no
                AND pld.bch = ld.bch
            LEFT JOIN webloan.dbo.loan_product AS lp
                ON ld.loan_product = lp.id_code
            LEFT JOIN webloan.dbo.loan_purpose AS lp2
                ON ld.cat_loan_purpose = lp2.path
            LEFT JOIN webloan.dbo.loan_acct_info AS la
                ON pld.acct_no = la.acct_no
                AND pld.bch = la.bch
            LEFT JOIN webloan.dbo.check_list_data AS cld
                ON la.cis_no = cld.cis_no
                AND cld.check_list_item = 'CCR07'
            WHERE pld.approved_date IS NULL
              AND pld.prepared_date IS NULL
              AND pld.released_date IS NULL
              AND pld.void_date IS NULL
              AND pld.bch = {branchCode}
              AND pld.acct_no = {accountNo}
            ORDER BY pld.bch, pld.acct_no, pld.loan_no";

        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.PendingLoanRows
            .FromSqlInterpolated(sql)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<LoanProductLookup>> GetActiveLoanProductsAsync(CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        return await context.LoanProducts
            .AsNoTracking()
            .Where(p => p.Expiration == null)
            .OrderBy(p => p.IdCode)
            .ToListAsync(ct);
    }
}