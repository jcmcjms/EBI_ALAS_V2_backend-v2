using Alas.Api.Composition;
using FluentValidation;

namespace Alas.Api.Features.ApprovalMatrix;

public static class ApprovalMatrixEndpoints
{
    public static void MapApprovalMatrixEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/approval-matrix").WithTags("Approval Matrix").RequireAuthorization();

        // Approval Authorities
        group.MapGet("/authorities", HandleGetAllAuthorities)
            .WithName("GetAllAuthorities");

        group.MapGet("/authorities/{key}", HandleGetAuthorityByKey)
            .WithName("GetAuthorityByKey");

        group.MapPost("/authorities", HandleCreateAuthority)
            .WithName("CreateAuthority")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/authorities/{key}", HandleUpdateAuthority)
            .WithName("UpdateAuthority")
            .ProducesValidationProblem();

        group.MapDelete("/authorities/{key}", HandleDeleteAuthority)
            .WithName("DeleteAuthority");

        // Deviation Catalog
        group.MapGet("/deviations", HandleGetAllDeviationCatalog)
            .WithName("GetAllDeviationCatalog");

        group.MapGet("/deviations/{id:int}", HandleGetDeviationCatalogById)
            .WithName("GetDeviationCatalogById");

        group.MapPost("/deviations", HandleCreateDeviationCatalog)
            .WithName("CreateDeviationCatalog")
            .ProducesValidationProblem();

        group.MapPut("/deviations/{id:int}", HandleUpdateDeviationCatalog)
            .WithName("UpdateDeviationCatalog")
            .ProducesValidationProblem();

        group.MapDelete("/deviations/{id:int}", HandleDeleteDeviationCatalog)
            .WithName("DeleteDeviationCatalog");
    }

    // Approval Authority handlers
    private static async Task<IResult> HandleGetAllAuthorities(
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var result = await service.GetAllAuthoritiesAsync(ct);
        return Results.Ok(ApiResponse<IReadOnlyList<ApprovalAuthorityResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetAuthorityByKey(
        string key,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var result = await service.GetAuthorityByKeyAsync(key, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<ApprovalAuthorityResponse>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleCreateAuthority(
        CreateApprovalAuthorityRequest request,
        IValidator<CreateApprovalAuthorityRequest> validator,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            var result = await service.CreateAuthorityAsync(request, ct);
            return Results.Created($"/api/approval-matrix/authorities/{result.Key}",
                ApiResponse<ApprovalAuthorityResponse>.SuccessResponse(result, "Authority created successfully"));
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

    private static async Task<IResult> HandleUpdateAuthority(
        string key,
        UpdateApprovalAuthorityRequest request,
        IValidator<UpdateApprovalAuthorityRequest> validator,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await service.UpdateAuthorityAsync(key, request, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<ApprovalAuthorityResponse>.SuccessResponse(result, "Authority updated successfully"));
    }

    private static async Task<IResult> HandleDeleteAuthority(
        string key,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var success = await service.DeleteAuthorityAsync(key, ct);
        return success
            ? Results.NoContent()
            : Results.NotFound();
    }

    // Deviation Catalog handlers
    private static async Task<IResult> HandleGetAllDeviationCatalog(
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var result = await service.GetAllDeviationCatalogAsync(ct);
        return Results.Ok(ApiResponse<IReadOnlyList<DeviationCatalogResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetDeviationCatalogById(
        int id,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var result = await service.GetDeviationCatalogByIdAsync(id, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<DeviationCatalogResponse>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleCreateDeviationCatalog(
        CreateDeviationCatalogRequest request,
        IValidator<CreateDeviationCatalogRequest> validator,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await service.CreateDeviationCatalogAsync(request, ct);
        return Results.Created($"/api/approval-matrix/deviations/{result.Id}",
            ApiResponse<DeviationCatalogResponse>.SuccessResponse(result, "Deviation catalog item created successfully"));
    }

    private static async Task<IResult> HandleUpdateDeviationCatalog(
        int id,
        UpdateDeviationCatalogRequest request,
        IValidator<UpdateDeviationCatalogRequest> validator,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await service.UpdateDeviationCatalogAsync(id, request, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<DeviationCatalogResponse>.SuccessResponse(result, "Deviation catalog item updated successfully"));
    }

    private static async Task<IResult> HandleDeleteDeviationCatalog(
        int id,
        IApprovalMatrixService service,
        CancellationToken ct)
    {
        var success = await service.DeleteDeviationCatalogAsync(id, ct);
        return success
            ? Results.NoContent()
            : Results.NotFound();
    }
}