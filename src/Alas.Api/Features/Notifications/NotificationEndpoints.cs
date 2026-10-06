using System.Security.Claims;
using Alas.Api.Composition;

namespace Alas.Api.Features.Notifications;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications")
            .RequireAuthorization();

        group.MapGet("/", HandleGetInbox)
            .WithName("GetNotificationInbox")
            .Produces<ApiResponse<InboxPage>>(200);

        group.MapGet("/recent", HandleGetRecent)
            .WithName("GetRecentNotifications")
            .Produces<ApiResponse<List<NotificationResponse>>>(200);

        group.MapPut("/read-all", HandleMarkAllRead)
            .WithName("MarkAllNotificationsRead")
            .Produces<ApiResponse<MarkAllReadResponse>>(200);

        group.MapPut("/{id:int}/read", HandleMarkRead)
            .WithName("MarkNotificationRead")
            .Produces<ApiResponse<MarkAllReadResponse>>(200)
            .Produces<ApiResponse<MarkAllReadResponse>>(404);
    }

    private static async Task<IResult> HandleGetInbox(
        ClaimsPrincipal principal,
        INotificationService service,
        int? page,
        int? pageSize,
        string? status,
        string? type,
        string? search,
        CancellationToken ct)
    {
        var userId = GetUserId(principal);
        var allowedStatus = (status ?? "all").ToLowerInvariant();
        if (allowedStatus is not ("unread" or "read"))
            allowedStatus = "all";

        var query = new InboxQuery(
            Page: Math.Max(1, page ?? 1),
            PageSize: Math.Clamp(pageSize ?? 10, 1, 50),
            Status: allowedStatus,
            Type: type,
            Search: search);

        var inbox = await service.GetInboxAsync(userId, query, ct);
        return Results.Ok(ApiResponse<InboxPage>.SuccessResponse(inbox));
    }

    private static async Task<IResult> HandleGetRecent(
        ClaimsPrincipal principal,
        INotificationService service,
        CancellationToken ct)
    {
        var userId = GetUserId(principal);
        var notifications = await service.GetUserNotificationsAsync(userId, ct: ct);
        return Results.Ok(ApiResponse<List<NotificationResponse>>.SuccessResponse(notifications));
    }

    private static async Task<IResult> HandleMarkAllRead(
        ClaimsPrincipal principal,
        INotificationService service,
        CancellationToken ct)
    {
        var userId = GetUserId(principal);
        var changed = await service.MarkAllReadAsync(userId, ct);
        return Results.Ok(ApiResponse<MarkAllReadResponse>.SuccessResponse(
            new MarkAllReadResponse(changed)));
    }

    private static async Task<IResult> HandleMarkRead(
        ClaimsPrincipal principal,
        int id,
        INotificationService service,
        CancellationToken ct)
    {
        var userId = GetUserId(principal);
        var found = await service.MarkReadAsync(userId, id, ct);
        return found
            ? Results.Ok(ApiResponse<MarkAllReadResponse>.SuccessResponse(
                new MarkAllReadResponse(1)))
            : Results.NotFound(ApiResponse<MarkAllReadResponse>.ErrorResponse(
                "Notification not found."));
    }

    private static int GetUserId(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst("userId");
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token");
        return userId;
    }
}