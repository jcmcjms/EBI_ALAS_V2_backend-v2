using Alas.Api.Composition;
using Alas.Api.Features.Loans.Domain;
using Alas.Api.Features.Users.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Loans;

public sealed class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;

    public LoanRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<LoanApplicationListResponse>> GetLoansAsync(LoanQueryParameters parameters, CancellationToken ct = default)
    {
        var query = _context.LoanApplications.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Status))
            query = query.Where(l => l.Status == parameters.Status);

        if (!string.IsNullOrWhiteSpace(parameters.BranchCode))
            query = query.Where(l => l.BranchCode == parameters.BranchCode);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search;
            query = query.Where(l =>
                l.FirstName.Contains(search) ||
                l.LastName.Contains(search) ||
                l.LoanNo.Contains(search) ||
                l.CisId!.Contains(search));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(l => l.ApplicationDate)
            .ThenBy(l => l.Id)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(l => new LoanApplicationListResponse(
                l.Id, l.LamId, l.BranchCode, l.FirstName, l.LastName,
                l.LoanNo, l.ProductCode, l.Product, l.ProposedAmount,
                l.Status, l.LoanType, l.ApplicationDate, l.LastActionDate))
            .ToListAsync(ct);

        return new PagedResult<LoanApplicationListResponse>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<LoanApplication?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.LoanApplications.FindAsync([id], ct);

    public async Task<LoanApplication?> GetByLamIdAsync(string lamId, CancellationToken ct = default) =>
        await _context.LoanApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LamId == lamId, ct);

    public async Task<LoanApplication?> GetByLoanNoAsync(string loanNo, CancellationToken ct = default) =>
        await _context.LoanApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LoanNo == loanNo, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        await _context.LoanApplications.AnyAsync(l => l.Id == id, ct);

    public async Task<LoanApplication> CreateAsync(LoanApplication loan, CancellationToken ct = default)
    {
        _context.LoanApplications.Add(loan);
        await _context.SaveChangesAsync(ct);
        return loan;
    }

    public async Task UpdateAsync(LoanApplication loan, CancellationToken ct = default)
    {
        _context.LoanApplications.Update(loan);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LoanProduct>> GetActiveProductsAsync(CancellationToken ct = default) =>
        await _context.LoanProducts
            .AsNoTracking()
            .Where(p => !p.IsRetired)
            .OrderBy(p => p.Code)
            .ToListAsync(ct);

    public async Task<LoanProduct?> GetProductByCodeAsync(string code, CancellationToken ct = default) =>
        await _context.LoanProducts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code == code, ct);

    public async Task<List<User>> GetUsersByRoleAndBranchAsync(
        string role, string branchCode, CancellationToken ct = default) =>
        await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == role && u.BranchId == branchCode && u.IsActive)
            .ToListAsync(ct);
}