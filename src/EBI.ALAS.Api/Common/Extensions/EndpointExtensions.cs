using EBI.ALAS.Api.Features.Auth;
using EBI.ALAS.Api.Features.Users;
using EBI.ALAS.Api.Features.Branches;
using EBI.ALAS.Api.Features.Loans;
using EBI.ALAS.Api.Features.ApprovalMatrix;
using EBI.ALAS.Api.Features.AuditLogs;
using EBI.ALAS.Api.Features.Notifications;
using EBI.ALAS.Api.Features.Presence;
using EBI.ALAS.Api.Features.Dashboard;
using EBI.ALAS.Api.Features.WebLoans;
using EBI.ALAS.Api.Features.SystemSettings;
using EBI.ALAS.Api.Features.Account;

namespace EBI.ALAS.Api.Common.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapBranchEndpoints();
        app.MapLoanEndpoints();
        app.MapApprovalMatrixEndpoints();
        app.MapAuditLogEndpoints();
        app.MapNotificationEndpoints();
        app.MapPresenceEndpoints();
        app.MapDashboardEndpoints();
        app.MapWebLoanEndpoints();
        app.MapSystemSettingsEndpoints();
        app.MapAccountEndpoints();

        // Health checks
        app.MapHealthChecks("/health").AllowAnonymous();

        // OpenAPI
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}