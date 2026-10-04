namespace EBI.ALAS.Api.Features.Loans;

public sealed record SubmitLoanApplicationRequest(
    ClientSection Client,
    BranchTypeSection BranchType,
    IReadOnlyList<LoanSection> Loans,
    IReadOnlyList<OutstandingLoanSection> OutstandingLoans,
    PreLoanSection? PreLoan);

public sealed record ClientSection(
    string CisId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    DateOnly? Birthdate,
    string? Address,
    string? Agency,
    string? Position,
    string? EmployeeId,
    decimal? NetTakeHomePay,
    string? LengthOfService,
    string? Region,
    string? DivisionCode,
    string? StationCode,
    string? MisAgency,
    string? School,
    string? Referrer);

public sealed record BranchTypeSection(
    string? RequestingOfficer,
    string? Lai);

public sealed record LoanSection(
    int? CreationTypeCode,
    string? CreationTypeLabel,
    string BranchCode,
    string LoanNo,
    string ProductCode,
    ParametersSection Parameters,
    DeviationsSection Deviations,
    IReadOnlyList<EbiReloanSection> EbiReloans,
    IReadOnlyList<BuyOutSection> BuyOuts,
    IReadOnlyList<IncomingLoanSection> IncomingLoans,
    decimal? CDocStamp);

public sealed record ParametersSection(
    string Product,
    string? Purpose,
    decimal ProposedAmount,
    int Term,
    decimal InterestRate,
    int? PolicyTermMonths,
    string? NthpDate,
    decimal NotarialFee,
    decimal DocStamps,
    decimal Insurance,
    StandardFeesSnapshot StandardFeesSnapshot);

public sealed record StandardFeesSnapshot(
    decimal NotarialFee,
    decimal DocStamps,
    decimal Insurance,
    decimal ApplicationCharge,
    decimal AdvanceInterest);

public sealed record DeviationsSection(
    bool HasDeviations,
    IReadOnlyList<string> DeviationDetails,
    IReadOnlyDictionary<string, string> DeviationJustifications,
    string? Remarks,
    string? AoRecommendation,
    string? OtherRemarks,
    string? FeeDeviationJustification);

public sealed record EbiReloanSection(
    string Pn,
    string Name,
    decimal ExistingDeduction,
    decimal OutstandingBalance,
    decimal PayToClose);

public sealed record BuyOutSection(
    string Pn,
    string Name,
    decimal Amortization,
    decimal OutstandingBalance);

public sealed record IncomingLoanSection(
    string Name,
    decimal Deductions,
    string? Remarks);

public sealed record OutstandingLoanSection(
    string Pn,
    decimal PrincipalBalance,
    decimal Amortization,
    decimal OutstandingBalance,
    string? DateGranted,
    string? DateMaturity,
    string? Status,
    string? ProductWithDescription);

public sealed record PreLoanSection(
    int Id,
    string FormNumber,
    string AccountNo,
    string Bch);

public sealed record LoanSubmissionResponse(
    string ApplicationGroupNo,
    IReadOnlyList<CreatedLoan> Loans);

public sealed record CreatedLoan(
    int Id,
    string LamId,
    string LoanNo,
    string ProductCode,
    decimal ProposedAmount,
    string Status);