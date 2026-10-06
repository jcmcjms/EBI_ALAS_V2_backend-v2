namespace Alas.Api.Composition.Constants;

public static class RolePermissions
{
    private static readonly Dictionary<string, string[]> RolePermissionMap = new()
    {
        [Roles.Encoder] =
        [
            Permissions.LoansCreate,
            Permissions.LoansView
        ],
        [Roles.Recommender] =
        [
            Permissions.LoansRecommend,
            Permissions.LoansView
        ],
        [Roles.Evaluator] =
        [
            Permissions.LoansEvaluate,
            Permissions.LoansView
        ],
        [Roles.Approver] =
        [
            Permissions.LoansApprove,
            Permissions.LoansReject,
            Permissions.LoansView
        ],
        [Roles.Admin] =
        [
            Permissions.LoansCreate,
            Permissions.LoansView,
            Permissions.LoansRecommend,
            Permissions.LoansEvaluate,
            Permissions.LoansApprove,
            Permissions.LoansReject,
            Permissions.LoanProductManage,
            Permissions.LoanProductView,
            Permissions.UserCreate,
            Permissions.UserView,
            Permissions.UserEdit,
            Permissions.UserSuspend,
            Permissions.RoleManage,
            Permissions.RoleView,
            Permissions.AuditLogsView,
            Permissions.WorkflowManage
        ]
    };

    public static string[] GetPermissionsForRole(string role) =>
        RolePermissionMap.TryGetValue(role, out var permissions)
            ? permissions
            : [];

    public static bool RoleHasPermission(string role, string permission)
    {
        if (!RolePermissionMap.TryGetValue(role, out var permissions))
            return false;

        if (role == Roles.Admin)
            return true;

        return permissions.Contains(permission);
    }
}