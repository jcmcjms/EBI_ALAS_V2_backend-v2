using Microsoft.AspNetCore.Authorization;
using EBI.ALAS.Api.Common.Constants;
using EBI.ALAS.Api.Shared.Authorization;

namespace EBI.ALAS.Api.Common.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("CanViewLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansView)))
            .AddPolicy("CanCreateLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansCreate)))
            .AddPolicy("CanRecommendLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansRecommend)))
            .AddPolicy("CanEvaluateLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansEvaluate)))
            .AddPolicy("CanApproveLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansApprove)))
            .AddPolicy("CanRejectLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansReject)))
            .AddPolicy("CanCancelLoan", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoansCancel)))
            .AddPolicy("CanManageLoanProduct", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoanProductManage)))
            .AddPolicy("CanViewLoanProduct", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.LoanProductView)))
            .AddPolicy("CanCreateUsers", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserCreate)))
            .AddPolicy("CanViewUsers", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserView)))
            .AddPolicy("CanEditUsers", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserEdit)))
            .AddPolicy("CanSuspendUsers", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserSuspend)))
            .AddPolicy("CanManageRoles", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.RoleManage)))
            .AddPolicy("CanViewRoles", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.RoleView)))
            .AddPolicy("CanViewAuditLogs", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.AuditLogsView)))
            .AddPolicy("CanManageWorkflow", policy => policy.Requirements.Add(new PermissionRequirement(Permissions.WorkflowManage)));

        return services;
    }
}