using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.Dashboard;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/overview", GetOverviewAsync)
            .WithName("GetDashboardOverview")
            .Produces<ApiResponse<DashboardOverviewDto>>(200);
    }

    private static async Task<IResult> GetOverviewAsync(
        IDashboardService dashboardService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var role = user.GetRole();
        var branchCode = user.GetBranchId();

        var result = await dashboardService.GetOverviewAsync(userId, role, branchCode, ct);
        return TypedResults.Ok(ApiResponse<DashboardOverviewDto>.SuccessResponse(result));
    }
}