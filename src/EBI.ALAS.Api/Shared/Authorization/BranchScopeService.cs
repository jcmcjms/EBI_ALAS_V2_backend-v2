using System.Security.Claims;
using EBI.ALAS.Api.Common.Constants;
using EBI.ALAS.Api.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Shared.Authorization;

public interface IBranchScopeService
{
    Task<IReadOnlyList<string>?> GetReadableBranchesAsync(ClaimsPrincipal user, CancellationToken ct = default);
}

public sealed class BranchScopeService(IUserRepository userRepository) : IBranchScopeService
{
    public async Task<IReadOnlyList<string>?> GetReadableBranchesAsync(ClaimsPrincipal user, CancellationToken ct = default)
    {
        var role = user.GetRole();
        var userId = user.GetUserId();

        // Admin sees all branches
        if (string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return null; // null means no filter
        }

        // Get user's branch
        var userEntity = await userRepository.GetByIdAsync(userId, ct);
        if (userEntity is null)
        {
            return [];
        }

        // Branch-scoped roles see only their branch
        var branchScopedRoles = new[] { Roles.Encoder, Roles.Recommender, Roles.Evaluator };
        if (branchScopedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            return [userEntity.BranchCode];
        }

        // Approvers see their area's branches
        if (string.Equals(role, Roles.Approver, StringComparison.OrdinalIgnoreCase))
        {
            // This would typically query the approval matrix for area coverage
            // For now, return the user's branch
            return [userEntity.BranchCode];
        }

        return [];
    }
}