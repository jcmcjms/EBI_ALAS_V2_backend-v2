namespace EBI.ALAS.Api.Features.Dashboard;

public sealed class DashboardService : IDashboardService
{
    public Task<DashboardOverviewDto> GetOverviewAsync(int userId, string role, string? branchCode, CancellationToken ct = default)
    {
        return Task.FromResult(new DashboardOverviewDto(0, 0, 0, 0, 0, [], []));
    }
}