using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Notifications;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications")
            .RequireAuthorization();

        group.MapGet("/", GetNotificationsAsync)
            .WithName("GetNotifications")
            .Produces<ApiResponse<IReadOnlyList<Notification>>>(200);
    }

    private static async Task<IResult> GetNotificationsAsync(
        int? limit, INotificationService notificationService, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = user.GetUserId();
        var notifications = await notificationService.GetRecentAsync(userId, limit ?? 20, ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<Notification>>.SuccessResponse(notifications));
    }
}