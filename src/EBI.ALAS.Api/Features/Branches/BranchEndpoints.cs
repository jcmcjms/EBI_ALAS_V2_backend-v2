using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.Branches;

public static class BranchEndpoints
{
    public static void MapBranchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/branches")
            .WithTags("Branches")
            .RequireAuthorization();

        group.MapGet("/", GetBranchesAsync)
            .RequireAuthorization("CanViewUsers")
            .WithName("GetBranches")
            .Produces<ApiResponse<IReadOnlyList<BranchDto>>>(200);

        group.MapGet("/all", GetAllBranchesAsync)
            .RequireAuthorization("CanViewUsers")
            .WithName("GetAllBranches")
            .Produces<ApiResponse<IReadOnlyList<BranchSimpleDto>>>(200);

        group.MapGet("/{id:int}", GetBranchByIdAsync)
            .RequireAuthorization("CanViewUsers")
            .WithName("GetBranchById")
            .Produces<ApiResponse<BranchDto>>(200)
            .Produces<ApiResponse>(404);

        group.MapGet("/code/{code}", GetBranchByCodeAsync)
            .RequireAuthorization("CanViewUsers")
            .WithName("GetBranchByCode")
            .Produces<ApiResponse<BranchDto>>(200)
            .Produces<ApiResponse>(404);
    }

    private static async Task<IResult> GetBranchesAsync(
        IBranchService branchService, CancellationToken ct)
    {
        var branches = await branchService.GetAllAsync(true, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<BranchDto>>.SuccessResponse(branches));
    }

    private static async Task<IResult> GetAllBranchesAsync(
        IBranchService branchService, CancellationToken ct)
    {
        var branches = await branchService.GetSimpleAsync(true, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<BranchSimpleDto>>.SuccessResponse(branches));
    }

    private static async Task<IResult> GetBranchByIdAsync(
        int id, IBranchService branchService, CancellationToken ct)
    {
        var branch = await branchService.GetByIdAsync(id, ct);
        if (branch is null)
        {
            return TypedResults.NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found", "BRANCH_NOT_FOUND"));
        }
        return TypedResults.Ok(ApiResponse<BranchDto>.SuccessResponse(branch));
    }

    private static async Task<IResult> GetBranchByCodeAsync(
        string code, IBranchService branchService, CancellationToken ct)
    {
        var branch = await branchService.GetByCodeAsync(code, ct);
        if (branch is null)
        {
            return TypedResults.NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found", "BRANCH_NOT_FOUND"));
        }
        return TypedResults.Ok(ApiResponse<BranchDto>.SuccessResponse(branch));
    }
}