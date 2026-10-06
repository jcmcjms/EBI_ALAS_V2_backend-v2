using Alas.Api.Features.Branches.Domain;
using Alas.Api.Composition;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Branches;

public sealed class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _context;

    public BranchRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<BranchListResponse>> GetBranchesAsync(BranchQueryParameters parameters, CancellationToken ct = default)
    {
        var query = _context.Branches.AsNoTracking().AsQueryable();

        if (parameters.IsActive.HasValue)
            query = query.Where(b => b.IsActive == parameters.IsActive.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.Code)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(b => new BranchListResponse(b.Id, b.Code, b.Name, b.IsActive))
            .ToListAsync(ct);

        return new PagedResult<BranchListResponse>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IReadOnlyList<BranchListResponse>> GetAllBranchesAsync(bool? isActive = null, CancellationToken ct = default)
    {
        var query = _context.Branches.AsNoTracking().AsQueryable();

        if (isActive.HasValue)
            query = query.Where(b => b.IsActive == isActive.Value);

        return await query
            .OrderBy(b => b.Code)
            .Select(b => new BranchListResponse(b.Id, b.Code, b.Name, b.IsActive))
            .ToListAsync(ct);
    }

    public async Task<Branch?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Branches.FindAsync([id], ct);

    public async Task<Branch?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Code == code, ct);

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default)
    {
        var query = _context.Branches.Where(b => b.Code == code);
        if (excludeId.HasValue) query = query.Where(b => b.Id != excludeId.Value);
        return await query.AnyAsync(ct);
    }

    public async Task AddAsync(Branch branch, CancellationToken ct = default)
    {
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public async Task DeleteAsync(Branch branch, CancellationToken ct = default)
    {
        _context.Branches.Remove(branch);
        await _context.SaveChangesAsync(ct);
    }
}