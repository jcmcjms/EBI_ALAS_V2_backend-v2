using Microsoft.AspNetCore.Authorization;

namespace EBI.ALAS.Api.Shared.Authorization;

public sealed class PermissionRequirement(string Permission) : IAuthorizationRequirement
{
    public string Permission { get; } = Permission;
}