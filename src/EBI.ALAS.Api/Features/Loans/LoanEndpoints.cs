using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using EBI.ALAS.Api.Shared.Models;
using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Loans;

public static class LoanEndpoints
{
    public static void MapLoanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/loans")
            .WithTags("Loans")
            .RequireAuthorization();

        group.MapGet("/", GetLoansAsync)
            .RequireAuthorization("CanViewLoan")
            .WithName("GetLoans")
            .Produces<ApiResponse<PagedResult<LoanApplication>>>(200);

        group.MapGet("/{id:int}", GetLoanAsync)
            .RequireAuthorization("CanViewLoan")
            .WithName("GetLoan")
            .Produces<ApiResponse<LoanApplication>>(200)
            .Produces<ApiResponse>(404);

        group.MapPost("/", CreateLoanAsync)
            .RequireAuthorization("CanCreateLoan")
            .WithName("CreateLoan")
            .Produces<ApiResponse<LoanApplication>>(201)
            .Produces<ApiResponse>(400);

        group.MapPut("/{id:int}/status", UpdateLoanStatusAsync)
            .RequireAuthorization("CanViewLoan")
            .WithName("UpdateLoanStatus")
            .Produces<ApiResponse<LoanApplication>>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(404);

        group.MapPost("/{id:int}/cancel", CancelLoanAsync)
            .RequireAuthorization("CanCreateLoan")
            .WithName("CancelLoan")
            .Produces<ApiResponse>(200)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(404);
    }

    private static async Task<IResult> GetLoansAsync(
        int? page, int? pageSize, string? search, string? status, string? branchCode,
        string? sortBy, bool? sortDesc, DateTime? fromDate, DateTime? toDate,
        ILoanRepository loanRepo, IBranchScopeService branchScope,
        ClaimsPrincipal user, CancellationToken ct)
    {
        var p = Math.Max(page ?? 1, 1);
        var ps = Math.Clamp(pageSize ?? 15, 1, 100);

        var readableBranches = await branchScope.GetReadableBranchesAsync(user, ct);

        var result = await loanRepo.GetPagedAsync(p, ps, search, status, branchCode, fromDate, toDate, sortBy, sortDesc ?? false, readableBranches, ct);

        return TypedResults.Ok(ApiResponse<PagedResult<LoanApplication>>.SuccessResponse(result));
    }

    private static async Task<IResult> GetLoanAsync(
        int id, ILoanRepository loanRepo, CancellationToken ct)
    {
        var loan = await loanRepo.GetByIdAsync(id, ct);
        if (loan is null)
        {
            return TypedResults.NotFound(ApiResponse<LoanApplication>.FailureResponse("Loan not found", "LOAN_NOT_FOUND"));
        }
        return TypedResults.Ok(ApiResponse<LoanApplication>.SuccessResponse(loan));
    }

    private static async Task<IResult> CreateLoanAsync(
        LoanApplication request, ILoanRepository loanRepo, CancellationToken ct)
    {
        var loan = await loanRepo.CreateAsync(request, ct);
        return TypedResults.Created($"/api/loans/{loan.Id}", ApiResponse<LoanApplication>.SuccessResponse(loan, "Loan created"));
    }

    private static async Task<IResult> UpdateLoanStatusAsync(
        int id, LoanStatusUpdateRequest request, ILoanRepository loanRepo, ILoanWorkflowService workflowService, CancellationToken ct)
    {
        var loan = await loanRepo.GetByIdAsync(id, ct);
        if (loan is null)
        {
            return TypedResults.NotFound(ApiResponse<LoanApplication>.FailureResponse("Loan not found", "LOAN_NOT_FOUND"));
        }

        // Validate transition
        var userRole = "Admin"; // Would come from context
        if (!workflowService.IsValidTransition(loan.Status, request.NewStatus, userRole))
        {
            return TypedResults.BadRequest(ApiResponse<LoanApplication>.FailureResponse(
                $"Invalid transition from {loan.Status} to {request.NewStatus}", "INVALID_TRANSITION"));
        }

        loan.Status = request.NewStatus;
        loan.LastActionDate = DateTime.UtcNow;
        await loanRepo.UpdateAsync(loan, ct);

        return TypedResults.Ok(ApiResponse<LoanApplication>.SuccessResponse(loan));
    }

    private static async Task<IResult> CancelLoanAsync(
        int id, ILoanRepository loanRepo, ILoanWorkflowService workflowService, ClaimsPrincipal user, CancellationToken ct)
    {
        var loan = await loanRepo.GetByIdAsync(id, ct);
        if (loan is null)
        {
            return TypedResults.NotFound(ApiResponse.FailureResponse("Loan not found", "LOAN_NOT_FOUND"));
        }

        var userId = user.GetUserId();
        if (loan.CreatedById != userId && !user.IsInRole(Roles.Admin))
        {
            return TypedResults.BadRequest(ApiResponse.FailureResponse("Can only cancel own loans", "FORBIDDEN"));
        }

        if (!workflowService.IsValidTransition(loan.Status, "Cancelled", user.GetRole()))
        {
            return TypedResults.BadRequest(ApiResponse.FailureResponse(
                $"Cannot cancel loan in status {loan.Status}", "INVALID_TRANSITION"));
        }

        loan.Status = "Cancelled";
        loan.LastActionDate = DateTime.UtcNow;
        await loanRepo.UpdateAsync(loan, ct);

        return TypedResults.Ok(ApiResponse.SuccessResponse("Loan cancelled"));
    }
}

public sealed record LoanStatusUpdateRequest(string NewStatus, string? Remarks);