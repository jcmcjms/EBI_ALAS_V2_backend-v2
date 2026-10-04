using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Features.WebLoans;

namespace EBI.ALAS.Api.Infrastructure.Data;

public sealed class WebLoanDbContext(DbContextOptions<WebLoanDbContext> options) : DbContext(options)
{
    public DbSet<CisInfo> CisInfo => Set<CisInfo>();
    public DbSet<CisInfoMiscData> CisInfoMiscData => Set<CisInfoMiscData>();
    public DbSet<MisGroup> MisGroups => Set<MisGroup>();
    public DbSet<LoanAcctInfo> LoanAcctInfos => Set<LoanAcctInfo>();
    public DbSet<LoanData> LoanData => Set<LoanData>();
    public DbSet<LoanProductLookup> LoanProducts => Set<LoanProductLookup>();
    public DbSet<LoanStatusLookup> LoanStatuses => Set<LoanStatusLookup>();
    public DbSet<LoanPurpose> LoanPurposes => Set<LoanPurpose>();
    public DbSet<CreationType> CreationTypes => Set<CreationType>();
    public DbSet<AmortData> AmortData => Set<AmortData>();
    public DbSet<OutstandingLoanRow> OutstandingLoans => Set<OutstandingLoanRow>();
    public DbSet<PendingLoanRow> PendingLoans => Set<PendingLoanRow>();
    public DbSet<CheckListData> ChecklistData => Set<CheckListData>();
    public DbSet<LoanData> LoanDatas => Set<LoanData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CisInfo>().ToTable("cis_info").HasKey(c => c.CisNo);
        modelBuilder.Entity<CisInfoMiscData>().ToTable("cis_info_misdata").HasKey(c => c.CisNo);
        modelBuilder.Entity<MisGroup>().ToTable("mis_group").HasKey(m => m.MisGroupId);
        modelBuilder.Entity<LoanAcctInfo>().ToTable("loan_acct_info").HasKey(l => l.AcctNo);
        modelBuilder.Entity<LoanData>().ToTable("loan_data").HasKey(l => l.PnNo);
        modelBuilder.Entity<LoanProductLookup>().ToTable("loan_product").HasKey(l => l.ProductCode);
        modelBuilder.Entity<LoanStatusLookup>().ToTable("loan_status").HasKey(l => l.StatusCode);
        modelBuilder.Entity<LoanPurpose>().ToTable("loan_purpose").HasKey(l => l.PurposeCode);
        modelBuilder.Entity<CreationType>().ToTable("creation_types").HasKey(c => c.CreationTypeCode);
        modelBuilder.Entity<AmortData>().ToTable("amort_data").HasKey(a => a.PnNo);
        modelBuilder.Entity<OutstandingLoanRow>().ToTable("loan_data").HasKey(o => o.Pn);
        modelBuilder.Entity<PendingLoanRow>().ToTable("loan_data").HasKey(p => p.Pn);
        modelBuilder.Entity<CheckListData>().ToTable("checklist_data").HasKey(c => c.Id);
        modelBuilder.Entity<LoanData>().ToTable("loan_data").HasKey(l => l.PnNo);
    }
}