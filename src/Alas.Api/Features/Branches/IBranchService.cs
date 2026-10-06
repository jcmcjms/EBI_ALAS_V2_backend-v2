using Alas.Api.Composition;

namespace Alas.Api.Features.Branches;

public interface IBranchService
{
    Task<PagedResult<BranchListResponse>> GetBranchesAsync(BranchQueryParameters parameters, CancellationToken ct = default);
    Task<IReadOnlyList<BranchListResponse>> GetAllBranchesAsync(bool? isActive = null, CancellationToken ct = default);
    Task<BranchResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<BranchResponse?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<BranchResponse> CreateAsync(CreateBranchRequest request, CancellationToken ct = default);
    Task<BranchResponse?> UpdateAsync(int id, UpdateBranchRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}