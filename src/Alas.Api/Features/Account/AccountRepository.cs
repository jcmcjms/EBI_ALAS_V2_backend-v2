using Alas.Api.Features.Auth.Domain;
using Alas.Api.Features.Loans.Domain;
using Alas.Api.Features.Users.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Account;

public sealed class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public AccountRepository(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task<AccountProfileResponse?> GetProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await _context.Users.FindAsync([userId], ct);
        if (user is null) return null;

        var stats = await GetStatsAsync(userId, ct);

        return new AccountProfileResponse(
            user.Id,
            user.Username,
            user.FirstName,
            user.MiddleName,
            user.LastName,
            user.BranchId,
            user.Role,
            user.Email,
            user.Phone,
            user.CreatedAt,
            user.PasswordChangedAt,
            stats);
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _context.Users.FindAsync([userId], ct);
        if (user is null) return false;

        user.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedSessionsResponse> GetActiveSessionsAsync(
        int userId, int? currentSessionId, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var now = _timeProvider.GetUtcNow();
        var query = _context.RefreshTokens
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > now)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new SessionResponse(
                t.Id,
                t.DeviceInfo ?? "Unknown Device",
                t.CreatedAt,
                t.ExpiresAt,
                currentSessionId.HasValue && t.Id == currentSessionId.Value))
            .ToListAsync(ct);

        return new PagedSessionsResponse(
            items, pageNumber, pageSize, totalCount, totalPages,
            pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<SessionRevokeResult> RevokeSessionAsync(
        int userId, int sessionId, int? currentSessionId, CancellationToken ct = default)
    {
        if (currentSessionId == sessionId)
            return SessionRevokeResult.CurrentSession;

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == sessionId && t.UserId == userId && !t.IsRevoked, ct);

        if (token is null)
            return SessionRevokeResult.NotFound;

        token.IsRevoked = true;
        await _context.SaveChangesAsync(ct);
        return SessionRevokeResult.Revoked;
    }

    public async Task<int> RevokeOtherSessionsAsync(int userId, int? currentSessionId, CancellationToken ct = default)
    {
        var now = _timeProvider.GetUtcNow();
        var query = _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > now);

        if (currentSessionId.HasValue)
            query = query.Where(t => t.Id != currentSessionId.Value);

        var tokens = await query.ToListAsync(ct);
        if (tokens.Count == 0) return 0;

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _context.SaveChangesAsync(ct);
        return tokens.Count;
    }

    public async Task<List<ActivityResponse>> GetRecentActivityAsync(int userId, int limit = 10, CancellationToken ct = default)
    {
        return await _context.LoanActions
            .AsNoTracking()
            .Where(a => a.ActionByUserId == userId)
            .OrderByDescending(a => a.ActionDate)
            .Take(limit)
            .Select(a => new ActivityResponse(
                a.Id,
                a.LoanApplication != null ? a.LoanApplication.LamId : string.Empty,
                a.Action,
                a.FromStatus,
                a.ToStatus,
                a.Comments,
                a.ActionDate,
                a.LoanApplication != null ? $"{a.LoanApplication.FirstName} {a.LoanApplication.LastName}" : string.Empty))
            .ToListAsync(ct);
    }

    public async Task<List<ProcessedLoanResponse>> GetProcessedLoansAsync(int userId, int limit = 10, CancellationToken ct = default)
    {
        return await _context.LoanApplications
            .AsNoTracking()
            .Where(l => l.CreatedById == userId)
            .OrderByDescending(l => l.ApplicationDate)
            .Take(limit)
            .Select(l => new ProcessedLoanResponse(
                l.Id,
                l.LamId,
                $"{l.FirstName} {l.LastName}",
                l.Status,
                l.ApplicationDate,
                l.ProposedAmount))
            .ToListAsync(ct);
    }

    public async Task<List<RecentClientResponse>> GetRecentClientsAsync(int userId, int limit = 5, CancellationToken ct = default)
    {
        return await _context.LoanApplications
            .AsNoTracking()
            .Where(l => l.CreatedById == userId && l.CisId != null)
            .OrderByDescending(l => l.ApplicationDate)
            .Take(limit)
            .Select(l => new RecentClientResponse(
                l.CisId!,
                $"{l.FirstName} {l.LastName}",
                l.Agency,
                l.ApplicationDate))
            .ToListAsync(ct);
    }

    private async Task<AccountStatsResponse> GetStatsAsync(int userId, CancellationToken ct = default)
    {
        var stats = await _context.LoanApplications
            .Where(l => l.CreatedById == userId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Pending = g.Count(l =>
                    l.Status == LoanStatus.Draft ||
                    l.Status == LoanStatus.ForRecommendation ||
                    l.Status == LoanStatus.ForChecking ||
                    l.Status == LoanStatus.ForApproval),
                Approved = g.Count(l => l.Status == LoanStatus.Approved)
            })
            .FirstOrDefaultAsync(ct);

        var totalLoans = stats?.Total ?? 0;
        var pendingLoans = stats?.Pending ?? 0;
        var approvedLoans = stats?.Approved ?? 0;
        var approvalRate = totalLoans > 0 ? (int)((double)approvedLoans / totalLoans * 100) : 0;

        return new AccountStatsResponse(totalLoans, pendingLoans, approvalRate);
    }
}