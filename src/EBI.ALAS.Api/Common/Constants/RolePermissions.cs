namespace EBI.ALAS.Api.Common.Constants;

public static class RolePermissions
{
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Matrix = new Dictionary<string, IReadOnlyList<string>>
    {
        [Roles.Encoder] =
        [
            Permissions.LoansCreate,
            Permissions.LoansView,
            Permissions.LoansCancel,
            Permissions.LoanProductView,
            Permissions.UserView,
            Permissions.RoleView
        ],
        [Roles.Recommender] =
        [
            Permissions.LoansView,
            Permissions.LoansRecommend,
            Permissions.LoanProductView,
            Permissions.UserView,
            Permissions.RoleView
        ],
        [Roles.Evaluator] =
        [
            Permissions.LoansView,
            Permissions.LoansEvaluate,
            Permissions.LoanProductView,
            Permissions.UserView,
            Permissions.RoleView
        ],
        [Roles.Approver] =
        [
            Permissions.LoansView,
            Permissions.LoansApprove,
            Permissions.LoansReject,
            Permissions.LoanProductView,
            Permissions.UserView,
            Permissions.RoleView
        ],
        [Roles.Admin] = Permissions.All
    };

    public static IReadOnlyList<string> GetPermissionsForRole(string role) =>
        Matrix.TryGetValue(role, out var permissions) ? permissions : [];
}