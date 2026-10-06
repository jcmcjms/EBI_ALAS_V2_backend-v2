using Alas.Api.Composition.Constants;
using Alas.Api.Composition;

namespace Alas.Api.Features.RoleManagement;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/roles")
            .WithTags("Roles")
            .RequireAuthorization("CanViewRoles");

        group.MapGet("/", HandleGetRoles)
            .WithName("GetRoles");

        group.MapGet("/matrix", HandleGetRoleMatrix)
            .WithName("GetRoleMatrix");

        group.MapGet("/permissions", HandleGetPermissions)
            .WithName("GetPermissions");
    }

    private static IResult HandleGetRoles()
    {
        var roles = new List<object>
        {
            new { Name = Roles.Encoder, DisplayName = Roles.DisplayNames.Encoder },
            new { Name = Roles.Recommender, DisplayName = Roles.DisplayNames.Recommender },
            new { Name = Roles.Evaluator, DisplayName = Roles.DisplayNames.Evaluator },
            new { Name = Roles.Approver, DisplayName = Roles.DisplayNames.Approver },
            new { Name = Roles.Admin, DisplayName = Roles.DisplayNames.Admin }
        };

        return Results.Ok(ApiResponse<object>.SuccessResponse(roles));
    }

    private static IResult HandleGetRoleMatrix()
    {
        var matrix = new List<object>
        {
            new
            {
                Role = Roles.Encoder,
                DisplayName = Roles.DisplayNames.Encoder,
                Permissions = RolePermissions.GetPermissionsForRole(Roles.Encoder)
            },
            new
            {
                Role = Roles.Recommender,
                DisplayName = Roles.DisplayNames.Recommender,
                Permissions = RolePermissions.GetPermissionsForRole(Roles.Recommender)
            },
            new
            {
                Role = Roles.Evaluator,
                DisplayName = Roles.DisplayNames.Evaluator,
                Permissions = RolePermissions.GetPermissionsForRole(Roles.Evaluator)
            },
            new
            {
                Role = Roles.Approver,
                DisplayName = Roles.DisplayNames.Approver,
                Permissions = RolePermissions.GetPermissionsForRole(Roles.Approver)
            },
            new
            {
                Role = Roles.Admin,
                DisplayName = Roles.DisplayNames.Admin,
                Permissions = RolePermissions.GetPermissionsForRole(Roles.Admin)
            }
        };

        return Results.Ok(ApiResponse<object>.SuccessResponse(matrix));
    }

    private static IResult HandleGetPermissions()
    {
        return Results.Ok(ApiResponse<string[]>.SuccessResponse(Permissions.All));
    }
}