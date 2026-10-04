using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using EBI.ALAS.Api.Features.Auth;
using EBI.ALAS.Api.Features.Account;

namespace EBI.ALAS.Api.Features.Account;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/account")
            .WithTags("Account")
            .RequireAuthorization();

        group.MapGet("/me", GetProfileAsync)
            .WithName("GetProfile")
            .Produces<ApiResponse<AccountProfileDto>>(200);

        group.MapPut("/me", UpdateProfileAsync)
            .WithName("UpdateProfile")
            .Produces<ApiResponse<AccountProfileDto>>(200)
            .Produces<ApiResponse>(400);

        group.MapGet("/me/sessions", GetSessionsAsync)
            .WithName("GetSessions")
            .Produces<ApiResponse<IReadOnlyList<SessionDto>>>(200);

        group.MapGet("/me/activity", GetActivityAsync)
            .WithName("GetActivity")
            .Produces<ApiResponse<IReadOnlyList<ActivityItemDto>>>(200);

        group.MapGet("/me/loans", GetProcessedLoansAsync)
            .WithName("GetProcessedLoans")
            .Produces<ApiResponse<IReadOnlyList<ProcessedLoanDto>>>(200);

        group.MapGet("/me/clients", GetRecentClientsAsync)
            .WithName("GetRecentClients")
            .Produces<ApiResponse<IReadOnlyList<RecentClientDto>>>(200);
    }

    private static async Task<IResult> GetProfileAsync(
        IAccountService accountService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await accountService.GetProfileAsync(userId, ct);
        if (result is null)
        {
            return TypedResults.NotFound(ApiResponse<AccountProfileDto>.FailureResponse("User not found", "USER_NOT_FOUND"));
        }
        return TypedResults.Ok(ApiResponse<AccountProfileDto>.SuccessResponse(result));
    }

    private static async Task<IResult> UpdateProfileAsync(
        UpdateProfileRequest request, IAccountService accountService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await accountService.UpdateProfileAsync(userId, request, ct);
        return TypedResults.Ok(ApiResponse<AccountProfileDto>.SuccessResponse(result));
    }

    private static async Task<IResult> GetSessionsAsync(
        IAccountService accountService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await accountService.GetSessionsAsync(userId, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<SessionDto>>.SuccessResponse(result));
    }

    private static async Task<IResult> GetActivityAsync(
        IAccountService accountService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await accountService.GetActivityAsync(userId, 20, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<ActivityItemDto>>.SuccessResponse(result));
    }

    private static async Task<IResult> GetProcessedLoansAsync(
        IAccountService accountService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await accountService.GetProcessedLoansAsync(userId, 10, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<ProcessedLoanDto>>.SuccessResponse(result));
    }

    private static async Task<IResult> GetRecentClientsAsync(
        IAccountService accountService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var result = await accountService.GetRecentClientsAsync(userId, 10, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<RecentClientDto>>.SuccessResponse(result));
    }
}