using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Branches;

public interface IBranchService
{
    Task<BranchDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<BranchDto?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<BranchDto>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<IReadOnlyList<BranchSimpleDto>> GetSimpleAsync(bool activeOnly = true, CancellationToken ct = default);
}

public sealed class BranchService(IBranchRepository branchRepository) : IBranchService
{
    public Task<BranchDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        branchRepository.GetByIdAsync(id, ct).ContinueWith(t => t.Result != null ? MapToDto(t.Result) : null, ct);

    public Task<BranchDto?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        branchRepository.GetByCodeAsync(code, ct).ContinueWith(t => t.Result != null ? MapToDto(t.Result) : null, ct);

    public async Task<IReadOnlyList<BranchDto>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var branches = await branchRepository.GetAllAsync(activeOnly, ct);
        return branches.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<BranchSimpleDto>> GetSimpleAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var branches = await branchRepository.GetAllAsync(activeOnly, ct);
        return branches.Select(b => new BranchSimpleDto(b.Code, b.Name)).ToList();
    }

    private static BranchDto MapToDto(Branch b) => new(
        b.Id, b.Code, b.Name, b.AreaCode, b.Region, b.Address, b.Phone, b.Email, b.IsActive);
}