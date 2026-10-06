using System.Globalization;
using Alas.Api.Features.WebLoans.Domain;

namespace Alas.Api.Features.WebLoans;

public sealed class WebLoanService : IWebLoanService
{
    private readonly IWebLoanRepository _repository;
    private readonly TimeProvider _timeProvider;

    public WebLoanService(IWebLoanRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<CisSearchResponse?> SearchByCisAsync(string cisNo, string? branchCode, CancellationToken ct = default)
    {
        var cis = await _repository.GetCisInfoAsync(cisNo, ct);
        if (cis is null) return null;

        var accounts = await _repository.GetAccountsByCisAsync(cisNo, branchCode, ct);
        var agencyType = await _repository.GetAgencyTypeAsync(cisNo, ct);
        var lengthOfServiceRow = await _repository.GetLengthOfServiceAsync(cisNo, ct);
        var lengthOfService = ComputeLengthOfService(lengthOfServiceRow?.Description);

        string? agencyDescription = null;
        if (agencyType?.ValueStr is { Length: > 0 } agencyCode)
        {
            var agencyGroup = await _repository.GetMisGroupByIdCodeAsync(agencyCode, ct);
            agencyDescription = agencyGroup?.Description;
        }

        var borrower = new BorrowerDto(
            CisNo: cis.CisNo,
            FirstName: cis.FirstName ?? string.Empty,
            MiddleName: cis.MiddleName,
            LastName: cis.LastName ?? string.Empty,
            Title: cis.Title,
            Appelation: cis.Appelation,
            BirthDate: ParseBirthDate(cis.BirthDateRaw),
            Address: BuildAddress(cis),
            AgencyType: agencyDescription,
            PositionTitle: cis.Occupation,
            Region: WebLoanRegions.Resolve(cis.RegionCode),
            RegionCode: cis.RegionCode,
            DivisionCode: cis.DivisionCode,
            StationCode: cis.StationCode,
            EmployeeNumber: cis.EmployeeNo,
            MisAgency: null,
            RequestingOfficer: null,
            LengthOfService: lengthOfService);

        var accountDtos = accounts.Select(a => new AccountDto(
            BankCode: a.BankCode,
            BranchCode: a.BranchCode,
            AccountNo: a.AccountNo,
            AccountId: WebLoanAccountId.Format(a.BranchCode, a.AccountNo),
            Name: a.Name,
            CreditLimit: a.CreditLimit,
            UsedCredit: a.UsedCredit,
            BorrowerType: a.BorrowerType
        )).ToList();

        return new CisSearchResponse(borrower, accountDtos);
    }

    public async Task<OutstandingLoansResponse?> GetOutstandingLoansAsync(
        string cisNo, string accountId, int pageSize = 50, int pageNumber = 1, CancellationToken ct = default)
    {
        var (branchCode, accountNo) = WebLoanAccountId.Parse(accountId);
        var belongs = await _repository.AccountBelongsToCisAsync(cisNo, branchCode, accountNo, ct);
        if (!belongs) return null;

        var rows = await _repository.GetOutstandingLoansAsync(branchCode, accountNo, pageSize, pageNumber, ct);
        var loans = rows
            .OrderByDescending(r => r.DateGranted ?? DateTimeOffset.MinValue)
            .Select(r =>
            {
                var status = WebLoanStatusResolver.Resolve(r.StatusCode);
                var statusLabel = WebLoanStatusResolver.Label(status);
                var productCode = r.ProductCode ?? string.Empty;
                var productWithDesc = (r.ProductWithDescription ?? string.Empty).TrimEnd();

                if (productWithDesc.EndsWith(" - ", StringComparison.Ordinal))
                    productWithDesc = productWithDesc[..^3];

                return new OutstandingLoanDto(
                    LoanNo: r.LoanNo,
                    Principal: r.Principal,
                    PrincipalBalance: r.PrincipalBalance,
                    AmortAmount: r.ComputedAmortAmount,
                    DateGranted: r.DateGranted,
                    DateMaturity: r.DateMaturity,
                    ProductCode: productCode,
                    ProductStatus: $"{productCode} - {statusLabel}",
                    ProductWithDescription: productWithDesc);
            })
            .ToList();

        return new OutstandingLoansResponse(
            CisNo: cisNo,
            AccountId: accountId,
            BranchCode: branchCode,
            AccountNo: accountNo,
            Loans: loans);
    }

    public async Task<PendingLoanResponse?> GetPendingLoanAsync(string cisNo, string accountId, CancellationToken ct = default)
    {
        var (branchCode, accountNo) = WebLoanAccountId.Parse(accountId);
        var belongs = await _repository.AccountBelongsToCisAsync(cisNo, branchCode, accountNo, ct);
        if (!belongs) return null;

        var rows = await _repository.GetPendingLoansAsync(branchCode, accountNo, ct);
        var nthpRow = rows.FirstOrDefault();

        var dtos = rows
            .GroupBy(r => r.LoanNo)
            .Select(g => g.First())
            .Select(r =>
            {
                var productWithDesc = (r.ProductWithDescription ?? string.Empty).TrimEnd();
                if (productWithDesc.EndsWith(" - ", StringComparison.Ordinal))
                    productWithDesc = productWithDesc[..^3];

                return new PendingLoanDto(
                    LoanNo: r.LoanNo ?? string.Empty,
                    Principal: r.Principal,
                    GrantedRate: r.GrantedRate,
                    TotalTermDays: r.TotalTermDays,
                    PolicyTermMonths: r.TotalAmortization,
                    ProductWithDescription: productWithDesc,
                    LoanPurpose: r.LoanPurpose,
                    CreationType: r.CreationType,
                    CreationTypeLabel: string.IsNullOrEmpty(r.CreationTypeLabel)
                        ? WebLoanStatusResolver.CreationTypeLabel(r.CreationType)
                        : r.CreationTypeLabel,
                    CDocStamp: r.CDocStamp);
            })
            .ToList();

        return new PendingLoanResponse(
            CisNo: cisNo,
            AccountId: accountId,
            BranchCode: branchCode,
            AccountNo: accountNo,
            Loans: dtos,
            Nthp: nthpRow?.Nthp,
            NthpDate: nthpRow?.NthpDate);
    }

    public async Task<IReadOnlyList<LoanProductDto>> GetActiveLoanProductsAsync(CancellationToken ct = default)
    {
        var rows = await _repository.GetActiveLoanProductsAsync(ct);
        return rows.Select(p => new LoanProductDto(p.IdCode, p.Description ?? string.Empty)).ToList();
    }

    private static DateTimeOffset? ParseBirthDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        string[] formats = ["yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy", "yyyy/MM/dd", "dd-MM-yyyy"];

        if (DateTime.TryParseExact(raw.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc));

        return DateTime.TryParse(raw.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var fallback)
            ? new DateTimeOffset(DateTime.SpecifyKind(fallback, DateTimeKind.Utc))
            : null;
    }

    private string? ComputeLengthOfService(string? rawHireDate)
    {
        if (string.IsNullOrWhiteSpace(rawHireDate)) return null;
        var hireDate = ParseBirthDate(rawHireDate);
        if (hireDate is null) return null;

        var now = _timeProvider.GetUtcNow();
        var totalMonths = Math.Max(0, ((now.Year - hireDate.Value.Year) * 12) + (now.Month - hireDate.Value.Month));
        var years = totalMonths / 12;
        var months = totalMonths % 12;
        return $"{years} years, {months} months";
    }

    private static string? BuildAddress(CisInfo cis)
    {
        var parts = new List<string?>
        {
            cis.Zip, cis.HouseStreet, cis.City,
            cis.StateProvince, cis.Barangay, cis.Village
        }
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .Select(p => p!.Trim())
        .ToList();

        return parts.Count == 0 ? null : string.Join(", ", parts);
    }
}