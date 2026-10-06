using Alas.Api.Composition;
using FluentValidation;

namespace Alas.Api.Features.Branches;

public static class BranchEndpoints
{
    public static void MapBranchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/branches").WithTags("Branches");

        group.MapGet("/", HandleGetBranches)
            .WithName("GetBranches")
            .RequireAuthorization();

        group.MapGet("/all", HandleGetAllBranches)
            .WithName("GetAllBranches")
            .RequireAuthorization();

        group.MapGet("/{id:int}", HandleGetBranchById)
            .WithName("GetBranchById")
            .RequireAuthorization();

        group.MapGet("/code/{code}", HandleGetBranchByCode)
            .WithName("GetBranchByCode")
            .RequireAuthorization();

        group.MapPost("/", HandleCreateBranch)
            .WithName("CreateBranch")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .RequireAuthorization();

        group.MapPut("/{id:int}", HandleUpdateBranch)
            .WithName("UpdateBranch")
            .ProducesValidationProblem()
            .RequireAuthorization();

        group.MapDelete("/{id:int}", HandleDeleteBranch)
            .WithName("DeleteBranch")
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleGetBranches(
        [AsParameters] BranchQueryParameters parameters,
        IBranchService branchService,
        CancellationToken ct)
    {
        var result = await branchService.GetBranchesAsync(parameters, ct);
        return Results.Ok(ApiResponse<PagedResult<BranchListResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetAllBranches(
        IBranchService branchService,
        CancellationToken ct,
        bool? isActive = null)
    {
        var result = await branchService.GetAllBranchesAsync(isActive, ct);
        return Results.Ok(ApiResponse<IReadOnlyList<BranchListResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetBranchById(
        int id,
        IBranchService branchService,
        CancellationToken ct)
    {
        var branch = await branchService.GetByIdAsync(id, ct);
        return branch is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<BranchResponse>.SuccessResponse(branch));
    }

    private static async Task<IResult> HandleGetBranchByCode(
        string code,
        IBranchService branchService,
        CancellationToken ct)
    {
        var branch = await branchService.GetByCodeAsync(code, ct);
        return branch is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<BranchResponse>.SuccessResponse(branch));
    }

    private static async Task<IResult> HandleCreateBranch(
        CreateBranchRequest request,
        IValidator<CreateBranchRequest> validator,
        IBranchService branchService,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            var branch = await branchService.CreateAsync(request, ct);
            return Results.Created($"/api/branches/{branch.Id}", ApiResponse<BranchResponse>.SuccessResponse(branch, "Branch created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Problem(
                title: "Conflict",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.10");
        }
    }

    private static async Task<IResult> HandleUpdateBranch(
        int id,
        UpdateBranchRequest request,
        IValidator<UpdateBranchRequest> validator,
        IBranchService branchService,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var branch = await branchService.UpdateAsync(id, request, ct);
        return branch is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<BranchResponse>.SuccessResponse(branch, "Branch updated successfully"));
    }

    private static async Task<IResult> HandleDeleteBranch(
        int id,
        IBranchService branchService,
        CancellationToken ct)
    {
        var success = await branchService.DeleteAsync(id, ct);
        return success
            ? Results.NoContent()
            : Results.NotFound();
    }
}