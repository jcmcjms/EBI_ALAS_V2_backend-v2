using Alas.Api.Features.Branches.Domain;
using Alas.Api.Composition;

namespace Alas.Api.Features.Branches;

public sealed class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly TimeProvider _timeProvider;

    public BranchService(IBranchRepository branchRepository, TimeProvider timeProvider)
    {
        _branchRepository = branchRepository;
        _timeProvider = timeProvider;
    }

    public async Task<PagedResult<BranchListResponse>> GetBranchesAsync(BranchQueryParameters parameters, CancellationToken ct = default) =>
        await _branchRepository.GetBranchesAsync(parameters, ct);

    public async Task<IReadOnlyList<BranchListResponse>> GetAllBranchesAsync(bool? isActive = null, CancellationToken ct = default) =>
        await _branchRepository.GetAllBranchesAsync(isActive, ct);

    public async Task<BranchResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var branch = await _branchRepository.GetByIdAsync(id, ct);
        if (branch is null) return null;
        return new BranchResponse(branch.Id, branch.Code, branch.Name, branch.IsActive, branch.CreatedAt, branch.AreaCode);
    }

    public async Task<BranchResponse?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var branch = await _branchRepository.GetByCodeAsync(code, ct);
        if (branch is null) return null;
        return new BranchResponse(branch.Id, branch.Code, branch.Name, branch.IsActive, branch.CreatedAt, branch.AreaCode);
    }

    public async Task<BranchResponse> CreateAsync(CreateBranchRequest request, CancellationToken ct = default)
    {
        if (await _branchRepository.CodeExistsAsync(request.Code, ct: ct))
            throw new InvalidOperationException("Branch code already exists");

        var branch = new Branch
        {
            Code = request.Code.ToUpperInvariant(),
            Name = request.Name,
            AreaCode = request.AreaCode,
            IsActive = true,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        await _branchRepository.AddAsync(branch, ct);
        return new BranchResponse(branch.Id, branch.Code, branch.Name, branch.IsActive, branch.CreatedAt, branch.AreaCode);
    }

    public async Task<BranchResponse?> UpdateAsync(int id, UpdateBranchRequest request, CancellationToken ct = default)
    {
        var branch = await _branchRepository.GetByIdAsync(id, ct);
        if (branch is null) return null;

        branch.Name = request.Name;
        branch.IsActive = request.IsActive;
        branch.AreaCode = request.AreaCode;

        await _branchRepository.UpdateAsync(ct);
        return new BranchResponse(branch.Id, branch.Code, branch.Name, branch.IsActive, branch.CreatedAt, branch.AreaCode);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var branch = await _branchRepository.GetByIdAsync(id, ct);
        if (branch is null) return false;

        await _branchRepository.DeleteAsync(branch, ct);
        return true;
    }
}