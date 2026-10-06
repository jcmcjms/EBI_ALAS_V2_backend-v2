using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.RateLimiting;
using Alas.Api.Composition.Extensions;
using Alas.Api.Features.Account;
using Alas.Api.Features.ApprovalMatrix;
using Alas.Api.Features.AuditLogs;
using Alas.Api.Features.Auth;
using Alas.Api.Features.Auth.Domain;
using Alas.Api.Features.Branches;
using Alas.Api.Features.Dashboard;
using Alas.Api.Features.Loans;
using Alas.Api.Features.Notifications;
using Alas.Api.Features.Presence;
using Alas.Api.Features.Users;
using Alas.Api.Features.WebLoans;
using Alas.Api.Infrastructure.Data;
using Alas.Api.Infrastructure.Data.WebLoan;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace Alas.Api.Composition;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Time
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        // JWT Settings with validation
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // CORS Settings with validation
        services.AddOptions<CorsSettings>()
            .Bind(configuration.GetSection("Cors"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // CORS policy — uses validated CorsSettings
        var corsSettings = configuration.GetSection("Cors").Get<CorsSettings>()!;
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // Admin seeder settings with validation
        services.AddOptions<AdminSettings>()
            .Bind(configuration.GetSection("Admin"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Garnet (Redis) Settings with validation
        services.AddOptions<GarnetSettings>()
            .Bind(configuration.GetSection("Garnet"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Admin seeder
        services.AddScoped<AdminSeeder>();

        // Authentication
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationPolicies();

        // StackExchange.Redis client for Garnet (Redis-compatible)
        var garnetSettings = configuration.GetSection("Garnet").Get<GarnetSettings>()!;
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
            var config = ConfigurationOptions.Parse(garnetSettings.ConnectionString);
            config.AbortOnConnectFail = false;
            config.ConnectTimeout = 5000;
            config.SyncTimeout = 3000;
            var multiplexer = ConnectionMultiplexer.Connect(config);
            multiplexer.ConnectionFailed += (_, e) => logger.LogError(e.Exception, "Redis connection failed: {Message}", e.Exception?.Message ?? "Unknown error");
            multiplexer.ConnectionRestored += (_, _) => logger.LogInformation("Redis connection restored");
            multiplexer.ErrorMessage += (_, e) => logger.LogError("Redis error: {Message}", e.Message);
            return multiplexer;
        });

        // Presence services (using StackExchange.Redis for distributed state)
        services.AddSingleton<IPresenceService, PresenceService>();
        services.AddSingleton<IEntityWatchService, EntityWatchService>();

        // Rate limiting — sliding window for login, fixed window for general endpoints
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("LoginLimiter", httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(60),
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));

            options.AddFixedWindowLimiter("GeneralLimiter", options =>
            {
                options.PermitLimit = 120;
                options.Window = TimeSpan.FromSeconds(60);
                options.QueueLimit = 0;
            });
        });

        // Auth services
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenRevocationRepository, TokenRevocationRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // User services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        // Branch services
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IBranchService, BranchService>();

        // WebLoan services (read-only connection to webloan database)
        var webLoanConnection = configuration.GetConnectionString("WebLoanConnection");
        if (string.IsNullOrWhiteSpace(webLoanConnection))
            throw new InvalidOperationException("ConnectionStrings:WebLoanConnection is required but not configured.");

        services.AddDbContextFactory<WebLoanDbContext>(options =>
            options.UseSqlServer(webLoanConnection));
        services.AddScoped<IWebLoanRepository, WebLoanRepository>();
        services.AddScoped<IWebLoanService, WebLoanService>();

        // Loan services
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ILoanService, LoanService>();

        // Account services
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountService, AccountService>();

        // Approval Matrix services
        services.AddScoped<IApprovalMatrixRepository, ApprovalMatrixRepository>();
        services.AddScoped<IApprovalMatrixService, ApprovalMatrixService>();

        // Audit Log services
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        // Dashboard services
        services.AddScoped<IDashboardService, DashboardService>();

        // Notification services
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<Program>();

        // Problem Details
        services.AddProblemDetails();

        // OpenTelemetry — traces for ASP.NET Core + SQL Client
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("Alas.Api")
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService("Alas.Api", serviceVersion: "1.0.0"))
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddConsoleExporter();
            });

        return services;
    }
}

public sealed class CorsSettings
{
    [Required]
    [MinLength(1)]
    public string[] AllowedOrigins { get; init; } = [];
}

public sealed class GarnetSettings
{
    [Required]
    public string ConnectionString { get; init; } = "localhost:6379,abortConnect=false,connectTimeout=5000,syncTimeout=3000";
}