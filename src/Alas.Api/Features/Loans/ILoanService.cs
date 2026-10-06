using Alas.Api.Composition;

namespace Alas.Api.Features.Loans;

public interface ILoanService
{
    Task<PagedResult<LoanApplicationListResponse>> GetLoansAsync(LoanQueryParameters parameters, CancellationToken ct = default);
    Task<LoanApplicationResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request, int userId, CancellationToken ct = default);
    Task<LoanApplicationResponse?> UpdateAsync(int id, UpdateLoanApplicationRequest request, CancellationToken ct = default);
    Task<LoanSubmissionResponse> SubmitAsync(SubmitLoanRequest request, int userId, CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(int id, string newStatus, int userId, string? comments = null, CancellationToken ct = default);
    Task<IReadOnlyList<LoanProductResponse>> GetActiveProductsAsync(CancellationToken ct = default);
}