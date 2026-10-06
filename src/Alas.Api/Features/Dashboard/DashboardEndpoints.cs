using System.Security.Claims;
using Alas.Api.Composition;

namespace Alas.Api.Features.Dashboard;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard").RequireAuthorization();

        group.MapGet("/overview", HandleGetOverview)
            .WithName("GetDashboardOverview");
    }

    private static async Task<IResult> HandleGetOverview(
        ClaimsPrincipal principal,
        IDashboardService dashboardService,
        CancellationToken ct)
    {
        var branchCode = principal.FindFirst("branchId")?.Value;
        var role = principal.FindFirst("role")?.Value;

        var overview = await dashboardService.GetOverviewAsync(branchCode, role, ct);
        return Results.Ok(ApiResponse<DashboardOverviewResponse>.SuccessResponse(overview));
    }
}