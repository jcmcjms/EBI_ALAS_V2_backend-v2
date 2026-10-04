using EBI.ALAS.Api.Shared.Models;
using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Loans;

public interface ILoanRepository
{
    Task<LoanApplication?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LoanApplication?> GetByLamIdAsync(string lamId, CancellationToken ct = default);
    Task<PagedResult<LoanApplication>> GetPagedAsync(
        int page, int pageSize, string? search, string? status, string? branchCode,
        DateTime? fromDate, DateTime? toDate, string? sortBy, bool sortDesc,
        IReadOnlyList<string>? readableBranches, CancellationToken ct = default);
    Task<LoanApplication> CreateAsync(LoanApplication application, CancellationToken ct = default);
    Task<IReadOnlyList<LoanApplication>> CreateRangeAsync(IReadOnlyList<LoanApplication> applications, CancellationToken ct = default);
    Task UpdateAsync(LoanApplication application, CancellationToken ct = default);
    Task<LoanSubmissionIdempotency?> GetIdempotencyRecordAsync(Guid key, int userId, CancellationToken ct = default);
    Task CreateSubmissionAsync(IReadOnlyList<LoanApplication> applications, LoanSubmissionIdempotency idempotency, CancellationToken ct = default);
    Task UpdateIdempotencyResponseAsync(LoanSubmissionIdempotency idempotency, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetUsersByRoleAndBranchAsync(string role, string branchCode, CancellationToken ct = default);
    Task<LoanAction?> GetLastActionAsync(int loanApplicationId, CancellationToken ct = default);
}