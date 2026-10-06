using System.Security.Claims;
using Alas.Api.Composition.Constants;

namespace Alas.Api.Composition.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst("userId");
        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }

    public static string GetUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("username")?.Value ?? string.Empty;
    }

    public static string GetFirstName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("firstName")?.Value ?? string.Empty;
    }

    public static string GetLastName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("lastName")?.Value ?? string.Empty;
    }

    public static string GetBranchId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("branchId")?.Value ?? string.Empty;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("role")?.Value
            ?? principal.FindFirst(ClaimTypes.Role)?.Value
            ?? string.Empty;
    }

    public static string[] GetPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll("permission")
            .Select(c => c.Value)
            .ToArray();
    }

    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
    {
        var role = principal.GetRole();
        if (role == Roles.Admin)
            return true;

        return principal.GetPermissions().Contains(permission);
    }

    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal.GetRole() == role;
    }
}