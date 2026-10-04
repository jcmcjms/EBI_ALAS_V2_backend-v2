using System.Security.Claims;

namespace EBI.ALAS.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst("uid") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst("role") ?? principal.FindFirst(ClaimTypes.Role);
        return claim?.Value ?? string.Empty;
    }

    public static string GetBranchId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst("branch");
        return claim?.Value ?? string.Empty;
    }

    public static string GetUsername(this ClaimsPrincipal principal)
    {
        return principal.Identity?.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal.IsInRole(role) || principal.FindFirst("role")?.Value == role;
    }

    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll("permissions").Select(c => c.Value);
    }
}