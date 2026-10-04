namespace EBI.ALAS.Api.Features.WebLoans;

public sealed record CisInfo(
    string CisNo,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string? Birthdate,
    string? Address,
    string? Agency,
    string? Position,
    string? EmployeeId,
    string? Region,
    string? DivisionCode,
    string? StationCode);

public sealed record CisInfoMiscData(
    string CisNo,
    string? MisAgency,
    string? School,
    string? Referrer);

public sealed record MisGroup(
    string MisGroupId,
    string MisGroupName);

public sealed record LoanAcctInfo(
    string AcctNo,
    string CisNo,
    string ProductCode,
    string StatusCode,
    decimal PrincipalBalance,
    decimal Amortization,
    DateOnly? DateGranted,
    DateOnly? DateMaturity);

public sealed record LoanData(
    string PnNo,
    string AcctNo,
    string ProductCode,
    string StatusCode,
    decimal PrincipalBalance,
    decimal Amortization,
    DateOnly? DateGranted,
    DateOnly? DateMaturity,
    string? BchCode);

public sealed record LoanProductLookup(
    string ProductCode,
    string ProductName,
    string? Description,
    decimal InterestRate);

public sealed record LoanStatusLookup(
    string StatusCode,
    string StatusDescription);

public sealed record LoanPurpose(
    string PurposeCode,
    string PurposeDescription);

public sealed record CreationType(
    int CreationTypeCode,
    string CreationTypeLabel);

public sealed record AmortData(
    string PnNo,
    int SeqNo,
    DateOnly DueDate,
    decimal Amortization,
    decimal Principal,
    decimal Interest,
    decimal Balance);

public sealed record OutstandingLoanRow(
    string Pn,
    decimal PrincipalBalance,
    decimal Amortization,
    decimal OutstandingBalance,
    DateOnly? DateGranted,
    DateOnly? DateMaturity,
    string? Status,
    string? ProductWithDescription);

public sealed record PendingLoanRow(
    string Pn,
    decimal PrincipalBalance,
    decimal Amortization,
    decimal OutstandingBalance,
    DateOnly? DateGranted,
    DateOnly? DateMaturity,
    string? Status,
    string? ProductWithDescription);

public sealed record CheckListData(
    int Id,
    string CisNo,
    string DocumentCode,
    string DocumentName,
    string Status,
    DateOnly? SubmittedDate);

public sealed record LoanStatus(
    string StatusCode,
    string StatusDescription);