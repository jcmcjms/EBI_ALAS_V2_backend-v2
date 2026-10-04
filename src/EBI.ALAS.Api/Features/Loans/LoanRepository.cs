using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Shared.Models;
using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanRepository(AppDbContext db) : ILoanRepository
{
    public Task<LoanApplication?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.LoanApplications
            .AsNoTracking()
            .Include(l => l.CreatedBy)
            .Include(l => l.AssignedApprover)
            .Include(l => l.DocumentChecklists)
            .Include(l => l.Actions)
                .ThenInclude(a => a.ActionByUser)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public Task<LoanApplication?> GetByLamIdAsync(string lamId, CancellationToken ct = default) =>
        db.LoanApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LamId == lamId, ct);

    public async Task<PagedResult<LoanApplication>> GetPagedAsync(
        int page, int pageSize, string? search, string? status, string? branchCode,
        DateTime? fromDate, DateTime? toDate, string? sortBy, bool sortDesc,
        IReadOnlyList<string>? readableBranches, CancellationToken ct = default)
    {
        var query = db.LoanApplications.AsNoTracking();

        if (readableBranches is not null)
        {
            query = query.Where(l => readableBranches.Contains(l.BranchCode));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(l =>
                l.ApplicationGroupNo.Contains(s) ||
                l.FirstName.Contains(s) ||
                l.LastName.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statuses = status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (statuses.Length > 0)
            {
                query = query.Where(l => statuses.Contains(l.Status));
            }
        }

        if (!string.IsNullOrWhiteSpace(branchCode) && !branchCode.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(l => l.BranchCode == branchCode);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.ApplicationDate >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(l => l.ApplicationDate <= endDate);
        }

        query = (sortBy?.ToLower(), sortDesc) switch
        {
            ("applicationdate", true) => query.OrderByDescending(l => l.ApplicationDate),
            ("applicationdate", false) => query.OrderBy(l => l.ApplicationDate),
            ("proposedamount", true) => query.OrderByDescending(l => l.ProposedAmount),
            ("proposedamount", false) => query.OrderBy(l => l.ProposedAmount),
            ("status", true) => query.OrderByDescending(l => l.Status),
            ("status", false) => query.OrderBy(l => l.Status),
            ("customername", true) => query.OrderByDescending(l => l.LastName),
            ("customername", false) => query.OrderBy(l => l.LastName),
            _ => query.OrderByDescending(l => l.ApplicationDate)
        };

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<LoanApplication>(items, totalCount, page, pageSize);
    }

    public async Task<LoanApplication> CreateAsync(LoanApplication application, CancellationToken ct = default)
    {
        db.LoanApplications.Add(application);
        await db.SaveChangesAsync(ct);
        return application;
    }

    public async Task<IReadOnlyList<LoanApplication>> CreateRangeAsync(IReadOnlyList<LoanApplication> applications, CancellationToken ct = default)
    {
        db.LoanApplications.AddRange(applications);
        await db.SaveChangesAsync(ct);
        return applications;
    }

    public async Task UpdateAsync(LoanApplication application, CancellationToken ct = default)
    {
        db.LoanApplications.Update(application);
        await db.SaveChangesAsync(ct);
    }

    public Task<LoanSubmissionIdempotency?> GetIdempotencyRecordAsync(Guid key, int userId, CancellationToken ct = default) =>
        db.LoanSubmissionIdempotencies
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.IdempotencyKey == key && i.UserId == userId, ct);

    public async Task CreateSubmissionAsync(
        IReadOnlyList<LoanApplication> applications,
        LoanSubmissionIdempotency idempotency,
        CancellationToken ct = default)
    {
        db.LoanApplications.AddRange(applications);
        db.LoanSubmissionIdempotencies.Add(idempotency);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateIdempotencyResponseAsync(LoanSubmissionIdempotency idempotency, CancellationToken ct = default)
    {
        db.LoanSubmissionIdempotencies.Update(idempotency);
        await db.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<User>> GetUsersByRoleAndBranchAsync(string role, string branchCode, CancellationToken ct = default) =>
        db.Users
            .AsNoTracking()
            .Where(u => u.Role == role && u.BranchCode == branchCode && u.IsActive)
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<User>)t.Result);

    public Task<LoanAction?> GetLastActionAsync(int loanApplicationId, CancellationToken ct = default) =>
        db.LoanActions
            .AsNoTracking()
            .Include(a => a.ActionByUser)
            .Where(a => a.LoanApplicationId == loanApplicationId)
            .OrderByDescending(a => a.ActionDate)
            .ThenByDescending(a => a.Id)
            .FirstOrDefaultAsync(ct);
}