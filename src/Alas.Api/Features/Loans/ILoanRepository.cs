using Alas.Api.Composition;
using Alas.Api.Features.Loans.Domain;
using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Loans;

public interface ILoanRepository
{
    Task<PagedResult<LoanApplicationListResponse>> GetLoansAsync(LoanQueryParameters parameters, CancellationToken ct = default);
    Task<LoanApplication?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LoanApplication?> GetByLamIdAsync(string lamId, CancellationToken ct = default);
    Task<LoanApplication?> GetByLoanNoAsync(string loanNo, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<LoanApplication> CreateAsync(LoanApplication loan, CancellationToken ct = default);
    Task UpdateAsync(LoanApplication loan, CancellationToken ct = default);
    Task<IReadOnlyList<LoanProduct>> GetActiveProductsAsync(CancellationToken ct = default);
    Task<LoanProduct?> GetProductByCodeAsync(string code, CancellationToken ct = default);
    Task<List<User>> GetUsersByRoleAndBranchAsync(string role, string branchCode, CancellationToken ct = default);
}