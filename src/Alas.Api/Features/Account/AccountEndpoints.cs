using System.Security.Claims;
using Alas.Api.Composition;
using FluentValidation;

namespace Alas.Api.Features.Account;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/account").WithTags("Account").RequireAuthorization();

        group.MapGet("/me", HandleGetProfile)
            .WithName("GetAccountProfile");

        group.MapPut("/me", HandleUpdateProfile)
            .WithName("UpdateAccountProfile")
            .ProducesValidationProblem();

        group.MapGet("/me/sessions", HandleGetSessions)
            .WithName("GetAccountSessions");

        group.MapDelete("/me/sessions/others", HandleRevokeOtherSessions)
            .WithName("RevokeOtherAccountSessions");

        group.MapDelete("/me/sessions/{id:int}", HandleRevokeSession)
            .WithName("RevokeAccountSession");

        group.MapGet("/me/activity", HandleGetActivity)
            .WithName("GetAccountActivity");

        group.MapGet("/me/loans", HandleGetLoans)
            .WithName("GetAccountLoans");

        group.MapGet("/me/clients", HandleGetClients)
            .WithName("GetAccountClients");
    }

    private static async Task<IResult> HandleGetProfile(
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        var profile = await accountService.GetProfileAsync(userId, ct);
        return profile is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<AccountProfileResponse>.SuccessResponse(profile));
    }

    private static async Task<IResult> HandleUpdateProfile(
        ClaimsPrincipal principal,
        UpdateProfileRequest request,
        IValidator<UpdateProfileRequest> validator,
        IAccountService accountService,
        CancellationToken ct)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var success = await accountService.UpdateProfileAsync(userId, request, ct);
        return success
            ? Results.Ok(ApiResponse.SuccessResponse("Profile updated successfully"))
            : Results.NotFound();
    }

    private static async Task<IResult> HandleGetSessions(
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        var sessions = await accountService.GetActiveSessionsAsync(userId, null, pageNumber, pageSize, ct);
        return Results.Ok(ApiResponse<PagedSessionsResponse>.SuccessResponse(sessions));
    }

    private static async Task<IResult> HandleRevokeOtherSessions(
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        var count = await accountService.RevokeOtherSessionsAsync(userId, null, ct);
        return Results.Ok(ApiResponse<RevokedSessionsResponse>.SuccessResponse(new RevokedSessionsResponse(count)));
    }

    private static async Task<IResult> HandleRevokeSession(
        int id,
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        var result = await accountService.RevokeSessionAsync(userId, id, null, ct);
        return result switch
        {
            SessionRevokeResult.Revoked => Results.Ok(ApiResponse.SuccessResponse("Session revoked successfully")),
            SessionRevokeResult.CurrentSession => Results.BadRequest(ApiResponse.ErrorResponse("Cannot revoke current session")),
            _ => Results.NotFound()
        };
    }

    private static async Task<IResult> HandleGetActivity(
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct,
        int limit = 10)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        limit = Math.Clamp(limit, 1, 50);
        var activity = await accountService.GetRecentActivityAsync(userId, limit, ct);
        return Results.Ok(ApiResponse<List<ActivityResponse>>.SuccessResponse(activity));
    }

    private static async Task<IResult> HandleGetLoans(
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct,
        int limit = 10)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        limit = Math.Clamp(limit, 1, 50);
        var loans = await accountService.GetProcessedLoansAsync(userId, limit, ct);
        return Results.Ok(ApiResponse<List<ProcessedLoanResponse>>.SuccessResponse(loans));
    }

    private static async Task<IResult> HandleGetClients(
        ClaimsPrincipal principal,
        IAccountService accountService,
        CancellationToken ct,
        int limit = 5)
    {
        if (!TryGetUserId(principal, out var userId))
            return Results.Unauthorized();

        limit = Math.Clamp(limit, 1, 20);
        var clients = await accountService.GetRecentClientsAsync(userId, limit, ct);
        return Results.Ok(ApiResponse<List<RecentClientResponse>>.SuccessResponse(clients));
    }

    private static bool TryGetUserId(ClaimsPrincipal principal, out int userId)
    {
        userId = 0;
        var claim = principal.FindFirst("userId");
        return claim is not null && int.TryParse(claim.Value, out userId);
    }
}