namespace Alas.Api.Features.Loans;

public interface ILoanApplicationFields
{
    string BranchCode { get; }
    string? CisId { get; }
    string FirstName { get; }
    string? MiddleName { get; }
    string LastName { get; }
    string? Suffix { get; }
    string? Address { get; }
    string? Agency { get; }
    string? Position { get; }
    string? EmployeeId { get; }
    decimal? NetTakeHomePay { get; }
    string? LengthOfService { get; }
    string? Region { get; }
    string LoanNo { get; }
    string ProductCode { get; }
    string Product { get; }
    string? Purpose { get; }
    decimal ProposedAmount { get; }
    int TermDays { get; }
    decimal InterestRate { get; }
    int? PolicyTermMonths { get; }
    decimal NotarialFee { get; }
    decimal DocStamps { get; }
    decimal Insurance { get; }
    int? CreationTypeCode { get; }
    string? CreationTypeLabel { get; }
    string LoanType { get; }
}

public record LoanQueryParameters(
    string? Status,
    string? BranchCode,
    string? Search,
    int PageNumber = 1,
    int PageSize = 20
);

public record CreateLoanApplicationRequest(
    string BranchCode,
    string CisId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string? Address,
    string? Agency,
    string? Position,
    string? EmployeeId,
    decimal? NetTakeHomePay,
    string? LengthOfService,
    string? Region,
    string LoanNo,
    string ProductCode,
    string Product,
    string? Purpose,
    decimal ProposedAmount,
    int TermDays,
    decimal InterestRate,
    int? PolicyTermMonths,
    decimal NotarialFee,
    decimal DocStamps,
    decimal Insurance,
    int? CreationTypeCode = null,
    string? CreationTypeLabel = null,
    string LoanType = "New"
) : ILoanApplicationFields;

public record UpdateLoanApplicationRequest(
    string? Purpose,
    decimal ProposedAmount,
    int TermDays,
    decimal InterestRate,
    int? PolicyTermMonths,
    decimal NotarialFee,
    decimal DocStamps,
    decimal Insurance,
    string? Remarks,
    string? AoRecommendation
);

public record LoanApplicationResponse(
    int Id,
    string LamId,
    string ApplicationGroupNo,
    string BranchCode,
    string? CisId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string? Address,
    string? Agency,
    string? Position,
    string? EmployeeId,
    decimal? NetTakeHomePay,
    string? LengthOfService,
    string? Region,
    string LoanNo,
    string ProductCode,
    string Product,
    string? Purpose,
    decimal ProposedAmount,
    int TermDays,
    decimal InterestRate,
    int? PolicyTermMonths,
    decimal NotarialFee,
    decimal DocStamps,
    decimal Insurance,
    decimal TotalDeductions,
    decimal GrossProceeds,
    decimal NetProceedsToClient,
    decimal TotalExposure,
    decimal? MonthlyAmortization,
    bool HasDeviations,
    string? Remarks,
    string? AoRecommendation,
    string Status,
    string LoanType,
    DateTimeOffset ApplicationDate,
    DateTimeOffset LastActionDate,
    int CreatedById,
    int? AssignedApproverId,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? DocumentsCompleteAt,
    int? CreationTypeCode = null,
    string? CreationTypeLabel = null
);

public record LoanApplicationListResponse(
    int Id,
    string LamId,
    string BranchCode,
    string FirstName,
    string LastName,
    string LoanNo,
    string ProductCode,
    string Product,
    decimal ProposedAmount,
    string Status,
    string LoanType,
    DateTimeOffset ApplicationDate,
    DateTimeOffset LastActionDate
);

public record LoanProductResponse(
    int Id,
    string Code,
    string Description,
    decimal MinAmount,
    decimal MaxAmount,
    int MinTermDays,
    int MaxTermDays,
    decimal NotarialFee,
    decimal DocStampFee,
    decimal InsuranceFee,
    decimal AdvanceInterestRate,
    decimal ApplicationChargeRate,
    string AmortizationMode,
    bool ChargeAdvanceInterest,
    bool IsRetired
);

public record SubmitLoanRequest(
    string BranchCode,
    string CisId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string? Address,
    string? Agency,
    string? Position,
    string? EmployeeId,
    decimal? NetTakeHomePay,
    string? LengthOfService,
    string? Region,
    string LoanNo,
    string ProductCode,
    string Product,
    string? Purpose,
    decimal ProposedAmount,
    int TermDays,
    decimal InterestRate,
    int? PolicyTermMonths,
    decimal NotarialFee,
    decimal DocStamps,
    decimal Insurance,
    int? CreationTypeCode = null,
    string? CreationTypeLabel = null,
    string LoanType = "New"
) : ILoanApplicationFields;

public record LoanSubmissionResponse(
    int Id,
    string LamId,
    string ApplicationGroupNo,
    string LoanNo,
    string ProductCode,
    decimal ProposedAmount,
    string Status
);