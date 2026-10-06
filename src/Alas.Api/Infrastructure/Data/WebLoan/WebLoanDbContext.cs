using Alas.Api.Features.WebLoans.Domain;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Infrastructure.Data.WebLoan;

/// <summary>
/// Read-only DbContext for the webloan database.
/// The webloan database is owned by the WebLoan system — this API may only query it.
/// </summary>
public sealed class WebLoanDbContext : DbContext
{
    public WebLoanDbContext(DbContextOptions<WebLoanDbContext> options) : base(options) { }

    public DbSet<CisInfo> CisInfos => Set<CisInfo>();
    public DbSet<CisInfoMiscData> CisInfoMiscDatas => Set<CisInfoMiscData>();
    public DbSet<LoanAcctInfo> LoanAcctInfos => Set<LoanAcctInfo>();
    public DbSet<LoanProductLookup> LoanProducts => Set<LoanProductLookup>();
    public DbSet<MisGroup> MisGroups => Set<MisGroup>();
    public DbSet<CheckListData> CheckListDatas => Set<CheckListData>();
    public DbSet<OutstandingLoanRow> OutstandingLoanRows => Set<OutstandingLoanRow>();
    public DbSet<PendingLoanRow> PendingLoanRows => Set<PendingLoanRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CisInfo>(entity =>
        {
            entity.HasKey(e => e.CisNo);
            entity.Property(e => e.CisNo).HasMaxLength(10);
        });

        modelBuilder.Entity<CisInfoMiscData>(entity =>
        {
            entity.HasKey(e => new { e.CisNo, e.IdCode });
            entity.HasIndex(e => e.CisNo);
            entity.Property(e => e.CisNo).HasMaxLength(10);
        });

        modelBuilder.Entity<LoanAcctInfo>(entity =>
        {
            entity.HasKey(e => new { e.BankCode, e.BranchCode, e.AccountNo });
            entity.HasIndex(e => e.CisNo);
        });

        modelBuilder.Entity<LoanProductLookup>(entity =>
        {
            entity.HasKey(e => e.IdCode);
        });

        modelBuilder.Entity<MisGroup>(entity =>
        {
            entity.HasKey(e => e.FrpId);
            entity.HasIndex(e => new { e.GroupNo, e.Path });
            entity.HasIndex(e => new { e.GroupNo, e.IdCode });
        });

        modelBuilder.Entity<CheckListData>(entity =>
        {
            entity.HasKey(e => new { e.CisNo, e.CheckListItem });
            entity.HasIndex(e => e.CisNo);
        });

        modelBuilder.Entity<OutstandingLoanRow>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<PendingLoanRow>(entity =>
        {
            entity.HasNoKey();
        });
    }

    public override int SaveChanges() =>
        throw new InvalidOperationException("WebLoanDbContext is READ-ONLY. The webloan database is owned by the WebLoan system; this API may only query it.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("WebLoanDbContext is READ-ONLY. The webloan database is owned by the WebLoan system; this API may only query it.");
}