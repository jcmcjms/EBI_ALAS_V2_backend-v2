namespace EBI.ALAS.Api.Features.Dashboard;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync(int userId, string role, string? branchCode, CancellationToken ct = default);
}

public sealed record DashboardOverviewDto(
    int TotalApplications,
    int PendingRecommendation,
    int PendingChecking,
    int PendingApproval,
    int PendingDisbursement,
    IReadOnlyList<BranchMetricDto> BranchMetrics,
    IReadOnlyList<TrendDto> Trends);

public sealed record BranchMetricDto(string BranchCode, string BranchName, int PendingCount, decimal TotalExposure);
public sealed record TrendDto(DateTime Date, int Count);