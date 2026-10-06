using System.Security.Claims;
using Alas.Api.Composition;
using Alas.Api.Features.Presence;

namespace Alas.Api.Features.Presence;

public static class PresenceEndpoints
{
    public static void MapPresenceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/presence")
            .WithTags("Presence")
            .RequireAuthorization();

        group.MapGet("/online", async (IPresenceService presence, ClaimsPrincipal principal) =>
        {
            var role = principal.FindFirst("role")?.Value ?? string.Empty;
            var branchCode = principal.FindFirst("branchCode")?.Value ?? string.Empty;

            var all = await presence.GetOnlineUsersAsync();
            if (role != "Admin" && !string.IsNullOrEmpty(branchCode))
                all = all.Where(e => e.User.BranchCode == branchCode).ToList();

            return Results.Ok(ApiResponse<IReadOnlyList<PresenceEntry>>.SuccessResponse(all));
        })
        .WithName("GetOnlineUsers")
        .Produces<ApiResponse<IReadOnlyList<PresenceEntry>>>();

        group.MapGet("/", async (string? userIds, IPresenceService presence, ClaimsPrincipal principal) =>
        {
            var role = principal.FindFirst("role")?.Value ?? string.Empty;

            var ids = (userIds ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
                .Where(i => i.HasValue)
                .Select(i => i!.Value)
                .Distinct()
                .Take(200)
                .ToList();

            var flags = new List<PresenceFlagResponse>();
            foreach (var id in ids)
            {
                var online = await presence.IsOnlineAsync(id);
                var connections = role == "Admin" ? await presence.ConnectionCountAsync(id) : (int?)null;
                flags.Add(new PresenceFlagResponse(id, online, connections));
            }

            return Results.Ok(ApiResponse<PresenceFlagsResponse>.SuccessResponse(
                new PresenceFlagsResponse(flags)));
        })
        .WithName("GetPresenceFlags")
        .Produces<ApiResponse<PresenceFlagsResponse>>();
    }

    private static int GetUserId(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst("userId");
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token");
        return userId;
    }
}