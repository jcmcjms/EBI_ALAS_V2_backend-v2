namespace Alas.Api.Features.Dashboard;

public sealed record DashboardOverviewResponse(
    DashboardKpis Kpis,
    IReadOnlyList<PendingQueueItemDto> PendingQueue,
    IReadOnlyList<ApprovedLoanItemDto> RecentApprovals,
    IReadOnlyList<PushBackItemDto> RecentPushBacks,
    IReadOnlyList<DailyTrendPointDto> WeeklyTrend,
    DateTimeOffset GeneratedAt);

public sealed record DashboardKpis(
    int TotalPending,
    int PendingDeltaFromYesterday,
    int ApprovedToday,
    int PushBacksToday,
    int ApprovedVsAvgPercent);

public sealed record PendingQueueItemDto(
    int Position,
    string LamId,
    string BranchCode,
    string Status,
    DateTimeOffset WaitingSince,
    string ClientName);

public sealed record ApprovedLoanItemDto(
    string LamId,
    string ClientName,
    string BranchCode,
    DateTimeOffset ApprovedAt);

public sealed record PushBackItemDto(
    int Position,
    string LamId,
    string BranchCode,
    string Reason,
    DateTimeOffset PushedBackAt);

public sealed record DailyTrendPointDto(
    string Day,
    int Approved,
    int PushBacks);