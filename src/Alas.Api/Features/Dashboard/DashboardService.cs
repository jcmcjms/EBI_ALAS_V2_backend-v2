using System.Globalization;
using Alas.Api.Features.Loans.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private static readonly string[] PendingStatuses =
        [LoanStatus.ForRecommendation, LoanStatus.ForChecking, LoanStatus.ForApproval];

    private const int QueueSize = 6;
    private const int ListSize = 5;

    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public DashboardService(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task<DashboardOverviewResponse> GetOverviewAsync(
        string? branchCode, string? role, CancellationToken ct = default)
    {
        var now = _timeProvider.GetUtcNow();
        var todayStart = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
        var yesterdayStart = todayStart.AddDays(-1);
        var weekStart = todayStart.AddDays(-6);

        // Base query with optional branch filter
        var loansQuery = _context.LoanApplications.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(branchCode) && role != "Admin")
            loansQuery = loansQuery.Where(l => l.BranchCode == branchCode);

        // KPIs
        var kpis = await ComputeKpisAsync(loansQuery, todayStart, yesterdayStart, weekStart, ct);

        // Pending queue
        var pendingQueue = await GetPendingQueueAsync(loansQuery, ct);

        // Recent approvals
        var recentApprovals = await GetRecentApprovalsAsync(loansQuery, todayStart, ct);

        // Recent pushbacks
        var recentPushBacks = await GetRecentPushBacksAsync(loansQuery, todayStart, ct);

        // Weekly trend — uses same branch filter
        var weeklyTrend = await GetWeeklyTrendAsync(loansQuery, weekStart, ct);

        return new DashboardOverviewResponse(kpis, pendingQueue, recentApprovals, recentPushBacks, weeklyTrend, now);
    }

    private async Task<DashboardKpis> ComputeKpisAsync(
        IQueryable<LoanApplication> loansQuery,
        DateTimeOffset todayStart,
        DateTimeOffset yesterdayStart,
        DateTimeOffset weekStart,
        CancellationToken ct)
    {
        var stats = await loansQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Pending = g.Count(l => PendingStatuses.Contains(l.Status)),
                SubmittedToday = g.Count(l => l.ApplicationDate >= todayStart),
                SubmittedYesterday = g.Count(l => l.ApplicationDate >= yesterdayStart && l.ApplicationDate < todayStart),
                ApprovedToday = g.Count(l => l.Status == LoanStatus.Approved && l.LastActionDate >= todayStart),
                PushBacksToday = g.Count(l => l.Status == LoanStatus.ForRevision && l.LastActionDate >= todayStart),
                ApprovedThisWeek = g.Count(l => l.Status == LoanStatus.Approved && l.LastActionDate >= weekStart),
            })
            .FirstOrDefaultAsync(ct);

        var totalPending = stats?.Pending ?? 0;
        var pendingDelta = (stats?.SubmittedToday ?? 0) - (stats?.SubmittedYesterday ?? 0);
        var approvedToday = stats?.ApprovedToday ?? 0;
        var pushBacksToday = stats?.PushBacksToday ?? 0;
        var approvedThisWeek = stats?.ApprovedThisWeek ?? 0;
        var dailyAvg = approvedThisWeek / 7.0;
        var vsAvg = dailyAvg > 0
            ? (int)Math.Round((approvedToday - dailyAvg) / dailyAvg * 100)
            : approvedToday > 0 ? 100 : 0;

        return new DashboardKpis(totalPending, pendingDelta, approvedToday, pushBacksToday, vsAvg);
    }

    private async Task<IReadOnlyList<PendingQueueItemDto>> GetPendingQueueAsync(
        IQueryable<LoanApplication> loansQuery, CancellationToken ct)
    {
        var items = await loansQuery
            .Where(l => PendingStatuses.Contains(l.Status))
            .OrderBy(l => l.LastActionDate)
            .Take(QueueSize)
            .Select(l => new PendingQueueItemDto(
                0,
                l.LamId,
                l.BranchCode,
                l.Status,
                l.LastActionDate,
                $"{l.FirstName} {l.LastName}"))
            .ToListAsync(ct);

        return items.Select((item, i) => item with { Position = i + 1 }).ToList();
    }

    private async Task<IReadOnlyList<ApprovedLoanItemDto>> GetRecentApprovalsAsync(
        IQueryable<LoanApplication> loansQuery, DateTimeOffset todayStart, CancellationToken ct)
    {
        return await loansQuery
            .Where(l => l.Status == LoanStatus.Approved && l.LastActionDate >= todayStart)
            .OrderByDescending(l => l.LastActionDate)
            .Take(ListSize)
            .Select(l => new ApprovedLoanItemDto(
                l.LamId,
                $"{l.FirstName} {l.LastName}",
                l.BranchCode,
                l.LastActionDate))
            .ToListAsync(ct);
    }

    private async Task<IReadOnlyList<PushBackItemDto>> GetRecentPushBacksAsync(
        IQueryable<LoanApplication> loansQuery, DateTimeOffset todayStart, CancellationToken ct)
    {
        var items = await loansQuery
            .Where(l => l.Status == LoanStatus.ForRevision && l.LastActionDate >= todayStart)
            .OrderByDescending(l => l.LastActionDate)
            .Take(ListSize)
            .Select(l => new PushBackItemDto(
                0,
                l.LamId,
                l.BranchCode,
                l.Remarks ?? "No reason recorded",
                l.LastActionDate))
            .ToListAsync(ct);

        return items.Select((item, i) => item with { Position = i + 1 }).ToList();
    }

    private async Task<IReadOnlyList<DailyTrendPointDto>> GetWeeklyTrendAsync(
        IQueryable<LoanApplication> loansQuery, DateTimeOffset weekStart, CancellationToken ct)
    {
        var loanIdSubquery = loansQuery.Select(l => l.Id);

        var trendData = await _context.LoanActions
            .AsNoTracking()
            .Where(a => loanIdSubquery.Contains(a.LoanApplicationId) &&
                a.ActionDate >= weekStart &&
                (a.ToStatus == LoanStatus.Approved || a.ToStatus == LoanStatus.ForRevision))
            .GroupBy(a => new { Day = a.ActionDate.Date, a.ToStatus })
            .Select(g => new
            {
                Day = g.Key.Day,
                ToStatus = g.Key.ToStatus ?? string.Empty,
                Count = g.Count()
            })
            .ToListAsync(ct);

        if (trendData.Count == 0)
            return GenerateEmptyTrend();

        var today = _timeProvider.GetUtcNow().Date;
        return Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var day = today.AddDays(-6 + offset);
                var label = day.ToString("ddd", CultureInfo.InvariantCulture);
                var approved = trendData
                    .Where(x => x.Day.Date == day && x.ToStatus == LoanStatus.Approved)
                    .Sum(x => x.Count);
                var pushBacks = trendData
                    .Where(x => x.Day.Date == day && x.ToStatus == LoanStatus.ForRevision)
                    .Sum(x => x.Count);
                return new DailyTrendPointDto(label, approved, pushBacks);
            })
            .ToList();
    }

    private IReadOnlyList<DailyTrendPointDto> GenerateEmptyTrend()
    {
        var today = _timeProvider.GetUtcNow().Date;
        return Enumerable.Range(0, 7)
            .Select(offset => new DailyTrendPointDto(
                today.AddDays(-6 + offset).ToString("ddd", CultureInfo.InvariantCulture),
                0, 0))
            .ToList();
    }
}