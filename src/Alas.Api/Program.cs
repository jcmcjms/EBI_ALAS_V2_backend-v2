using Alas.Api.Composition;
using Alas.Api.Composition.Extensions;
using Alas.Api.Features.Account;
using Alas.Api.Features.ApprovalMatrix;
using Alas.Api.Features.AuditLogs;
using Alas.Api.Features.Auth;
using Alas.Api.Features.Branches;
using Alas.Api.Features.Dashboard;
using Alas.Api.Features.Loans;
using Alas.Api.Features.Notifications;
using Alas.Api.Features.Presence;
using Alas.Api.Features.RoleManagement;
using Alas.Api.Features.Users;
using Alas.Api.Features.WebLoans;
using Alas.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Request body size limit — 1MB for API endpoints
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 1_048_576);

builder.Services.AddBankingCaching(builder.Configuration);
builder.Services.AddBankingMessaging(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddBankingHealthChecks(builder.Configuration);

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed admin user
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await seeder.SeedAsync();
}

// Global exception handler — prevents leaking stack traces, SQL, or internals
app.UseExceptionHandler(app =>
{
    app.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        if (exceptionHandlerFeature?.Error is not null)
            logger.LogError(exceptionHandlerFeature.Error, "Unhandled exception");

        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            title = "An unexpected error occurred.",
            status = 500
        });
    });
});

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    if (app.Environment.IsDevelopment())
    {
        // Relaxed CSP for Scalar API docs in development
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data:; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'");
    }
    else
    {
        // Strict CSP for production — API-only, no rendering
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'none'; frame-ancestors 'none'");
        context.Response.Headers.Append("Strict-Transport-Security",
            "max-age=63072000; includeSubDomains; preload");
    }

    await next();
});

// API Documentation (dev only)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseStatusCodePages();
app.UseRateLimiter();
app.UseCors("AllowFrontend");
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapRoleEndpoints();
app.MapBranchEndpoints();
app.MapWebLoanEndpoints();
app.MapLoanEndpoints();
app.MapAccountEndpoints();
app.MapApprovalMatrixEndpoints();
app.MapAuditLogEndpoints();
app.MapDashboardEndpoints();
app.MapNotificationEndpoints();
app.MapPresenceEndpoints();
app.MapHub<NotificationHub>("/hubs/notifications");

// Health endpoints
app.MapGet("/health/live", () => Results.Ok(new { status = "healthy" }))
    .ExcludeFromDescription();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration
            }),
            totalDuration = report.TotalDuration
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.Run();