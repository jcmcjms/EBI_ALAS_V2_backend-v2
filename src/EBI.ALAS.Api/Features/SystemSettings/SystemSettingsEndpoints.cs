using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EBI.ALAS.Api.Features.SystemSettings;

public static class SystemSettingsEndpoints
{
    public static void MapSystemSettingsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/system-settings")
            .WithTags("SystemSettings")
            .RequireAuthorization();

        group.MapGet("/", GetSettingsAsync)
            .RequireAuthorization("CanManageWorkflow")
            .WithName("GetSystemSettings")
            .Produces<ApiResponse<IReadOnlyList<SystemSetting>>>(200);
    }

    private static async Task<IResult> GetSettingsAsync(
        ISystemSettingsStore settingsStore, CancellationToken ct)
    {
        var settings = await settingsStore.GetAllAsync(ct);
        return TypedResults.Ok(ApiResponse<IReadOnlyList<SystemSetting>>.SuccessResponse(settings));
    }
}