namespace EBI.ALAS.Api.Features.Branches;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Branch?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Branch>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<Branch> CreateAsync(Branch branch, CancellationToken ct = default);
    Task UpdateAsync(Branch branch, CancellationToken ct = default);
}