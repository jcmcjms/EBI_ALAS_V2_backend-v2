using Alas.Api.Features.ApprovalMatrix.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.ApprovalMatrix;

public sealed class ApprovalMatrixRepository : IApprovalMatrixRepository
{
    private readonly AppDbContext _context;

    public ApprovalMatrixRepository(AppDbContext context) => _context = context;

    // Approval Authorities
    public async Task<IReadOnlyList<ApprovalAuthority>> GetAllAuthoritiesAsync(CancellationToken ct = default) =>
        await _context.ApprovalAuthorities
            .AsNoTracking()
            .OrderBy(a => a.Tier)
            .ThenBy(a => a.Priority)
            .ToListAsync(ct);

    public async Task<ApprovalAuthority?> GetAuthorityByKeyAsync(string key, CancellationToken ct = default) =>
        await _context.ApprovalAuthorities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Key == key, ct);

    public async Task<bool> AuthorityKeyExistsAsync(string key, CancellationToken ct = default) =>
        await _context.ApprovalAuthorities.AnyAsync(a => a.Key == key, ct);

    public async Task AddAuthorityAsync(ApprovalAuthority authority, CancellationToken ct = default)
    {
        _context.ApprovalAuthorities.Add(authority);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SaveAuthorityAsync(ApprovalAuthority authority, CancellationToken ct = default)
    {
        _context.ApprovalAuthorities.Update(authority);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAuthorityAsync(string key, CancellationToken ct = default)
    {
        var authority = await _context.ApprovalAuthorities.FindAsync([key], ct);
        if (authority is null) return false;

        _context.ApprovalAuthorities.Remove(authority);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    // Deviation Catalog
    public async Task<IReadOnlyList<DeviationCatalogItem>> GetAllDeviationCatalogAsync(CancellationToken ct = default) =>
        await _context.DeviationCatalog
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync(ct);

    public async Task<DeviationCatalogItem?> GetDeviationCatalogByIdAsync(int id, CancellationToken ct = default) =>
        await _context.DeviationCatalog.FindAsync([id], ct);

    public async Task AddDeviationCatalogAsync(DeviationCatalogItem item, CancellationToken ct = default)
    {
        _context.DeviationCatalog.Add(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SaveDeviationCatalogAsync(DeviationCatalogItem item, CancellationToken ct = default)
    {
        _context.DeviationCatalog.Update(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteDeviationCatalogAsync(int id, CancellationToken ct = default)
    {
        var item = await _context.DeviationCatalog.FindAsync([id], ct);
        if (item is null) return false;

        _context.DeviationCatalog.Remove(item);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}