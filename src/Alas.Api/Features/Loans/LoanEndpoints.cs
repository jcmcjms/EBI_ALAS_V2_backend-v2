using System.Security.Claims;
using Alas.Api.Composition;
using FluentValidation;

namespace Alas.Api.Features.Loans;

public static class LoanEndpoints
{
    public static void MapLoanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/loans").WithTags("Loans").RequireAuthorization();

        group.MapGet("/", HandleGetLoans)
            .WithName("GetLoans");

        group.MapGet("/{id:int}", HandleGetLoanById)
            .WithName("GetLoanById");

        group.MapPost("/", HandleCreateLoan)
            .WithName("CreateLoan")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", HandleUpdateLoan)
            .WithName("UpdateLoan")
            .ProducesValidationProblem();

        group.MapPost("/submit", HandleSubmitLoan)
            .WithName("SubmitLoan")
            .ProducesValidationProblem();

        group.MapPatch("/{id:int}/status", HandleUpdateStatus)
            .WithName("UpdateLoanStatus");

        group.MapGet("/products", HandleGetProducts)
            .WithName("GetLoanProducts");
    }

    private static async Task<IResult> HandleGetLoans(
        [AsParameters] LoanQueryParameters parameters,
        ILoanService loanService,
        CancellationToken ct)
    {
        var result = await loanService.GetLoansAsync(parameters, ct);
        return Results.Ok(ApiResponse<PagedResult<LoanApplicationListResponse>>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetLoanById(
        int id,
        ILoanService loanService,
        CancellationToken ct)
    {
        var loan = await loanService.GetByIdAsync(id, ct);
        return loan is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<LoanApplicationResponse>.SuccessResponse(loan));
    }

    private static async Task<IResult> HandleCreateLoan(
        CreateLoanApplicationRequest request,
        IValidator<CreateLoanApplicationRequest> validator,
        ILoanService loanService,
        ClaimsPrincipal principal,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var userId = GetUserId(principal);
        var loan = await loanService.CreateAsync(request, userId, ct);
        return Results.Created($"/api/loans/{loan.Id}", ApiResponse<LoanApplicationResponse>.SuccessResponse(loan, "Loan application created successfully"));
    }

    private static async Task<IResult> HandleUpdateLoan(
        int id,
        UpdateLoanApplicationRequest request,
        IValidator<UpdateLoanApplicationRequest> validator,
        ILoanService loanService,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var loan = await loanService.UpdateAsync(id, request, ct);
        return loan is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<LoanApplicationResponse>.SuccessResponse(loan, "Loan application updated successfully"));
    }

    private static async Task<IResult> HandleSubmitLoan(
        SubmitLoanRequest request,
        IValidator<SubmitLoanRequest> validator,
        ILoanService loanService,
        ClaimsPrincipal principal,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var userId = GetUserId(principal);
        var result = await loanService.SubmitAsync(request, userId, ct);
        return Results.Created($"/api/loans/{result.Id}", ApiResponse<LoanSubmissionResponse>.SuccessResponse(result, "Loan application submitted successfully"));
    }

    private static async Task<IResult> HandleUpdateStatus(
        int id,
        UpdateStatusRequest request,
        ILoanService loanService,
        ClaimsPrincipal principal,
        CancellationToken ct)
    {
        var userId = GetUserId(principal);
        var success = await loanService.UpdateStatusAsync(id, request.Status, userId, request.Comments, ct);
        return success
            ? Results.Ok(ApiResponse.SuccessResponse($"Loan status updated to {request.Status}"))
            : Results.BadRequest(ApiResponse.ErrorResponse("Invalid status transition or loan not found"));
    }

    private static async Task<IResult> HandleGetProducts(
        ILoanService loanService,
        CancellationToken ct)
    {
        var products = await loanService.GetActiveProductsAsync(ct);
        return Results.Ok(ApiResponse<IReadOnlyList<LoanProductResponse>>.SuccessResponse(products));
    }

    private static int GetUserId(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst("userId");
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token");
        return userId;
    }
}

public sealed record UpdateStatusRequest(string Status, string? Comments);