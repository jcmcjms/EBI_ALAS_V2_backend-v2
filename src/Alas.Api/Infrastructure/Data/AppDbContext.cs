using Alas.Api.Features.ApprovalMatrix.Domain;
using Alas.Api.Features.AuditLogs;
using Alas.Api.Features.Auth.Domain;
using Alas.Api.Features.Branches.Domain;
using Alas.Api.Features.Loans.Domain;
using Alas.Api.Features.Notifications.Domain;
using Alas.Api.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    public DbSet<LoanAction> LoanActions => Set<LoanAction>();
    public DbSet<ApprovalAuthority> ApprovalAuthorities => Set<ApprovalAuthority>();
    public DbSet<DeviationCatalogItem> DeviationCatalog => Set<DeviationCatalogItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}