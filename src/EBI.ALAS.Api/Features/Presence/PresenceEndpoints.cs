using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Presence;

public static class PresenceEndpoints
{
    public static void MapPresenceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/presence")
            .WithTags("Presence")
            .RequireAuthorization();

        group.MapGet("/online", GetOnlineUsersAsync)
            .WithName("GetOnlineUsers")
            .Produces<ApiResponse<IReadOnlyList<OnlineUserDto>>>(200);

        group.MapGet("/", GetPresenceBatchAsync)
            .WithName("GetPresenceBatch")
            .Produces<ApiResponse<IReadOnlyList<PresenceDto>>>(200);
    }

    private static async Task<IResult> GetOnlineUsersAsync(
        IPresenceService presenceService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var role = user.GetRole();
        var branchCode = user.GetBranchId();
        var isAdmin = string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase);

        var result = await presenceService.GetOnlineUsersAsync(userId, branchCode, isAdmin, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<OnlineUserDto>>.SuccessResponse(result));
    }

    private static async Task<IResult> GetPresenceBatchAsync(
        int[] userIds, IPresenceService presenceService, CancellationToken ct)
    {
        var result = await presenceService.GetPresenceAsync(userIds, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<PresenceDto>>.SuccessResponse(result));
    }
}