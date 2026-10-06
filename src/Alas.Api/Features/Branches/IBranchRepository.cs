using Alas.Api.Features.Branches.Domain;
using Alas.Api.Composition;

namespace Alas.Api.Features.Branches;

public interface IBranchRepository
{
    Task<PagedResult<BranchListResponse>> GetBranchesAsync(BranchQueryParameters parameters, CancellationToken ct = default);
    Task<IReadOnlyList<BranchListResponse>> GetAllBranchesAsync(bool? isActive = null, CancellationToken ct = default);
    Task<Branch?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Branch?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Branch branch, CancellationToken ct = default);
    Task UpdateAsync(CancellationToken ct = default);
    Task DeleteAsync(Branch branch, CancellationToken ct = default);
}