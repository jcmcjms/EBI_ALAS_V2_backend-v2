using System.Security.Claims;
using Alas.Api.Composition;

namespace Alas.Api.Features.WebLoans;

public static class WebLoanEndpoints
{
    public static void MapWebLoanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/webloans").WithTags("WebLoans").RequireAuthorization();

        group.MapGet("/cis/{cisNo}/search", HandleSearchCis)
            .WithName("SearchCis");

        group.MapGet("/cis/{cisNo}/accounts/{accountId}/outstanding-loans", HandleGetOutstandingLoans)
            .WithName("GetOutstandingLoans");

        group.MapGet("/cis/{cisNo}/accounts/{accountId}/pending-loan", HandleGetPendingLoan)
            .WithName("GetPendingLoan");

        group.MapGet("/loan-products", HandleGetLoanProducts)
            .WithName("GetActiveLoanProducts");
    }

    private static async Task<IResult> HandleSearchCis(
        string cisNo,
        IWebLoanService webLoanService,
        CancellationToken ct)
    {
        var result = await webLoanService.SearchByCisAsync(cisNo, null, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<CisSearchResponse>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetOutstandingLoans(
        string cisNo,
        string accountId,
        IWebLoanService webLoanService,
        CancellationToken ct,
        int pageSize = 50,
        int pageNumber = 1)
    {
        pageSize = Math.Clamp(pageSize, 1, 500);
        pageNumber = Math.Max(1, pageNumber);

        var result = await webLoanService.GetOutstandingLoansAsync(cisNo, accountId, pageSize, pageNumber, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<OutstandingLoansResponse>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetPendingLoan(
        string cisNo,
        string accountId,
        IWebLoanService webLoanService,
        CancellationToken ct)
    {
        var result = await webLoanService.GetPendingLoanAsync(cisNo, accountId, ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<PendingLoanResponse>.SuccessResponse(result));
    }

    private static async Task<IResult> HandleGetLoanProducts(
        IWebLoanService webLoanService,
        CancellationToken ct)
    {
        var products = await webLoanService.GetActiveLoanProductsAsync(ct);
        return Results.Ok(ApiResponse<IReadOnlyList<LoanProductDto>>.SuccessResponse(products));
    }
}