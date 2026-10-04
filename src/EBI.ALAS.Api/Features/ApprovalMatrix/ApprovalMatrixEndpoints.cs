using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public static class ApprovalMatrixEndpoints
{
    public static void MapApprovalMatrixEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/approval-matrix")
            .WithTags("ApprovalMatrix")
            .RequireAuthorization();

        group.MapGet("/", GetAuthoritiesAsync)
            .RequireAuthorization("CanViewLoan")
            .WithName("GetAuthorities")
            .Produces<ApiResponse<IReadOnlyList<ApprovalAuthority>>>(200);

        group.MapGet("/approvers", GetAvailableApproversAsync)
            .RequireAuthorization("CanViewLoan")
            .WithName("GetAvailableApprovers")
            .Produces<ApiResponse<IReadOnlyList<ApproverDto>>>(200);
    }

    private static async Task<IResult> GetAuthoritiesAsync(
        IApprovalRoutingService routingService, CancellationToken ct)
    {
        var authorities = await routingService.GetAuthoritiesAsync(ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<ApprovalAuthority>>.SuccessResponse(authorities));
    }

    private static async Task<IResult> GetAvailableApproversAsync(
        IApprovalRoutingService routingService, CancellationToken ct)
    {
        return TypedResults.Ok(ApiResponse<IReadOnlyList<ApproverDto>>.SuccessResponse([]));
    }
}