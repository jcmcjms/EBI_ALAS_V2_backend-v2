using System.Security.Cryptography;
using Alas.Api.Composition;
using Alas.Api.Features.Loans.Domain;

namespace Alas.Api.Features.Loans;

public sealed class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly TimeProvider _timeProvider;

    public LoanService(ILoanRepository loanRepository, TimeProvider timeProvider)
    {
        _loanRepository = loanRepository;
        _timeProvider = timeProvider;
    }

    public async Task<PagedResult<LoanApplicationListResponse>> GetLoansAsync(LoanQueryParameters parameters, CancellationToken ct = default) =>
        await _loanRepository.GetLoansAsync(parameters, ct);

    public async Task<LoanApplicationResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var loan = await _loanRepository.GetByIdAsync(id, ct);
        if (loan is null) return null;
        return MapToResponse(loan);
    }

    public async Task<LoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request, int userId, CancellationToken ct = default)
    {
        var loan = BuildLoanApplication(request, userId, LoanStatus.Draft);
        await _loanRepository.CreateAsync(loan, ct);
        return MapToResponse(loan);
    }

    public async Task<LoanApplicationResponse?> UpdateAsync(int id, UpdateLoanApplicationRequest request, CancellationToken ct = default)
    {
        var loan = await _loanRepository.GetByIdAsync(id, ct);
        if (loan is null) return null;

        loan.Purpose = request.Purpose;
        loan.ProposedAmount = request.ProposedAmount;
        loan.TermDays = request.TermDays;
        loan.InterestRate = request.InterestRate;
        loan.PolicyTermMonths = request.PolicyTermMonths;
        loan.NotarialFee = request.NotarialFee;
        loan.DocStamps = request.DocStamps;
        loan.Insurance = request.Insurance;
        loan.Remarks = request.Remarks;
        loan.AoRecommendation = request.AoRecommendation;
        loan.TotalDeductions = request.NotarialFee + request.DocStamps + request.Insurance;
        loan.GrossProceeds = request.ProposedAmount;
        loan.NetProceedsToClient = request.ProposedAmount - loan.TotalDeductions;
        loan.TotalExposure = request.ProposedAmount;
        loan.LastActionDate = _timeProvider.GetUtcNow();

        await _loanRepository.UpdateAsync(loan, ct);
        return MapToResponse(loan);
    }

    public async Task<LoanSubmissionResponse> SubmitAsync(SubmitLoanRequest request, int userId, CancellationToken ct = default)
    {
        var loan = BuildLoanApplication(request, userId, LoanStatus.ForRecommendation);
        await _loanRepository.CreateAsync(loan, ct);

        return new LoanSubmissionResponse(
            loan.Id,
            loan.LamId,
            loan.ApplicationGroupNo,
            loan.LoanNo,
            loan.ProductCode,
            loan.ProposedAmount,
            loan.Status);
    }

    private LoanApplication BuildLoanApplication(ILoanApplicationFields request, int userId, string status)
    {
        var now = _timeProvider.GetUtcNow();
        var totalDeductions = request.NotarialFee + request.DocStamps + request.Insurance;

        return new LoanApplication
        {
            LamId = GenerateLamId(),
            ApplicationGroupNo = GenerateApplicationGroupNo(),
            BranchCode = request.BranchCode,
            CisId = request.CisId,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Suffix = request.Suffix,
            Address = request.Address,
            Agency = request.Agency,
            Position = request.Position,
            EmployeeId = request.EmployeeId,
            NetTakeHomePay = request.NetTakeHomePay,
            LengthOfService = request.LengthOfService,
            Region = request.Region,
            LoanNo = request.LoanNo,
            ProductCode = request.ProductCode,
            Product = request.Product,
            Purpose = request.Purpose,
            ProposedAmount = request.ProposedAmount,
            TermDays = request.TermDays,
            InterestRate = request.InterestRate,
            PolicyTermMonths = request.PolicyTermMonths,
            NotarialFee = request.NotarialFee,
            DocStamps = request.DocStamps,
            Insurance = request.Insurance,
            TotalDeductions = totalDeductions,
            GrossProceeds = request.ProposedAmount,
            NetProceedsToClient = request.ProposedAmount - totalDeductions,
            TotalExposure = request.ProposedAmount,
            Status = status,
            LoanType = request.LoanType,
            CreationTypeCode = request.CreationTypeCode,
            CreationTypeLabel = request.CreationTypeLabel,
            ApplicationDate = now,
            LastActionDate = now,
            CreatedById = userId
        };
    }

    public async Task<bool> UpdateStatusAsync(int id, string newStatus, int userId, string? comments = null, CancellationToken ct = default)
    {
        if (!LoanStatus.IsValid(newStatus))
            return false;

        var loan = await _loanRepository.GetByIdAsync(id, ct);
        if (loan is null) return false;

        loan.Status = newStatus;
        loan.LastActionDate = _timeProvider.GetUtcNow();

        await _loanRepository.UpdateAsync(loan, ct);
        return true;
    }

    public async Task<IReadOnlyList<LoanProductResponse>> GetActiveProductsAsync(CancellationToken ct = default)
    {
        var products = await _loanRepository.GetActiveProductsAsync(ct);
        return products.Select(p => new LoanProductResponse(
            p.Id, p.Code, p.Description, p.MinAmount, p.MaxAmount,
            p.MinTermDays, p.MaxTermDays, p.NotarialFee, p.DocStampFee,
            p.InsuranceFee, p.AdvanceInterestRate, p.ApplicationChargeRate,
            p.AmortizationMode, p.ChargeAdvanceInterest, p.IsRetired
        )).ToList();
    }

    private static LoanApplicationResponse MapToResponse(LoanApplication loan) =>
        new(
            loan.Id,
            loan.LamId,
            loan.ApplicationGroupNo,
            loan.BranchCode,
            loan.CisId,
            loan.FirstName,
            loan.MiddleName,
            loan.LastName,
            loan.Suffix,
            loan.Address,
            loan.Agency,
            loan.Position,
            loan.EmployeeId,
            loan.NetTakeHomePay,
            loan.LengthOfService,
            loan.Region,
            loan.LoanNo,
            loan.ProductCode,
            loan.Product,
            loan.Purpose,
            loan.ProposedAmount,
            loan.TermDays,
            loan.InterestRate,
            loan.PolicyTermMonths,
            loan.NotarialFee,
            loan.DocStamps,
            loan.Insurance,
            loan.TotalDeductions,
            loan.GrossProceeds,
            loan.NetProceedsToClient,
            loan.TotalExposure,
            loan.MonthlyAmortization,
            loan.HasDeviations,
            loan.Remarks,
            loan.AoRecommendation,
            loan.Status,
            loan.LoanType,
            loan.ApplicationDate,
            loan.LastActionDate,
            loan.CreatedById,
            loan.AssignedApproverId,
            loan.AssignedAt,
            loan.DocumentsCompleteAt,
            loan.CreationTypeCode,
            loan.CreationTypeLabel);

    private string GenerateLamId()
    {
        var timestamp = _timeProvider.GetUtcNow().ToString("yyyyMMddHHmmss");
        var random = RandomNumberGenerator.GetInt32(1000, 9999);
        return $"LAM-{timestamp}-{random}";
    }

    private string GenerateApplicationGroupNo()
    {
        var timestamp = _timeProvider.GetUtcNow().ToString("yyyyMMdd");
        var random = RandomNumberGenerator.GetInt32(10000, 99999);
        return $"GRP-{timestamp}-{random}";
    }
}