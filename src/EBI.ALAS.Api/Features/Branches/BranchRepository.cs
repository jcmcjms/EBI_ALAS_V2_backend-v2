using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Features.Branches;

public sealed class BranchRepository(AppDbContext db) : IBranchRepository
{
    public Task<Branch?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<Branch?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Code == code, ct);

    public Task<IReadOnlyList<Branch>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var query = db.Branches.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(b => b.IsActive);
        }
        return query.OrderBy(b => b.Code).ToListAsync(ct).ContinueWith(t => (IReadOnlyList<Branch>)t.Result);
    }

    public async Task<Branch> CreateAsync(Branch branch, CancellationToken ct = default)
    {
        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);
        return branch;
    }

    public async Task UpdateAsync(Branch branch, CancellationToken ct = default)
    {
        db.Branches.Update(branch);
        await db.SaveChangesAsync(ct);
    }
}