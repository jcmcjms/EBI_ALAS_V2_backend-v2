using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.WebLoans;

public static class WebLoanEndpoints
{
    public static void MapWebLoanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/webloans")
            .WithTags("WebLoans")
            .RequireAuthorization();

        group.MapGet("/cis/{cisNo}/search", SearchCisAsync)
            .WithName("SearchCis")
            .Produces<ApiResponse<CisSearchResult>>(200);

        group.MapGet("/cis/{cisNo}", GetCisAsync)
            .WithName("GetCis")
            .Produces<ApiResponse<CisInfo>>(200);

        group.MapGet("/cis/{cisNo}/accounts/{accountNo}", GetAccountAsync)
            .WithName("GetAccount")
            .Produces<ApiResponse<LoanAcctInfo>>(200);
    }

    private static async Task<IResult> SearchCisAsync(
        string cisNo, IWebLoanService webLoanService, CancellationToken ct)
    {
        var result = await webLoanService.SearchAsync(cisNo, ct);
        return TypedResults.Ok(ApiResponse<CisSearchResult>.SuccessResponse(result));
    }

    private static async Task<IResult> GetCisAsync(
        string cisNo, IWebLoanService webLoanService, CancellationToken ct)
    {
        var result = await webLoanService.GetCisAsync(cisNo, ct);
        if (result is null)
        {
            return TypedResults.NotFound(ApiResponse<CisInfo>.FailureResponse("CIS not found", "CIS_NOT_FOUND"));
        }
        return TypedResults.Ok(ApiResponse<CisInfo>.SuccessResponse(result));
    }

    private static async Task<IResult> GetAccountAsync(
        string cisNo, string accountNo, IWebLoanService webLoanService, CancellationToken ct)
    {
        var result = await webLoanService.GetAccountAsync(cisNo, accountNo, ct);
        if (result is null)
        {
            return TypedResults.NotFound(ApiResponse<LoanAcctInfo>.FailureResponse("Account not found", "ACCOUNT_NOT_FOUND"));
        }
        return TypedResults.Ok(ApiResponse<LoanAcctInfo>.SuccessResponse(result));
    }
}