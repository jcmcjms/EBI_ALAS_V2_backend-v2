using Alas.Api.Features.Loans.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.LamId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.LamId)
            .IsUnique();

        builder.Property(e => e.ApplicationGroupNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.BranchCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.CisId)
            .HasMaxLength(10);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.MiddleName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Suffix)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.Agency)
            .HasMaxLength(100);

        builder.Property(e => e.Position)
            .HasMaxLength(100);

        builder.Property(e => e.EmployeeId)
            .HasMaxLength(50);

        builder.Property(e => e.LengthOfService)
            .HasMaxLength(50);

        builder.Property(e => e.Region)
            .HasMaxLength(50);

        builder.Property(e => e.LoanNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.LoanNo);

        builder.Property(e => e.ProductCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Product)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Purpose)
            .HasMaxLength(500);

        builder.Property(e => e.Remarks)
            .HasMaxLength(2000);

        builder.Property(e => e.AoRecommendation)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Status);

        builder.Property(e => e.LoanType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.CreationTypeLabel)
            .HasMaxLength(50);

        // Decimal precision for financial fields
        builder.Property(e => e.ProposedAmount).HasPrecision(18, 2);
        builder.Property(e => e.InterestRate).HasPrecision(10, 4);
        builder.Property(e => e.NetTakeHomePay).HasPrecision(18, 2);
        builder.Property(e => e.NotarialFee).HasPrecision(18, 2);
        builder.Property(e => e.DocStamps).HasPrecision(18, 2);
        builder.Property(e => e.Insurance).HasPrecision(18, 2);
        builder.Property(e => e.TotalDeductions).HasPrecision(18, 2);
        builder.Property(e => e.GrossProceeds).HasPrecision(18, 2);
        builder.Property(e => e.NetProceedsToClient).HasPrecision(18, 2);
        builder.Property(e => e.TotalExposure).HasPrecision(18, 2);
        builder.Property(e => e.MonthlyAmortization).HasPrecision(18, 2);

        builder.HasIndex(e => new { e.BranchCode, e.Status })
            .HasDatabaseName("IX_LoanApplications_BranchCode_Status");

        builder.HasIndex(e => new { e.CreatedById, e.Status })
            .HasDatabaseName("IX_LoanApplications_CreatedById_Status");

        builder.HasIndex(e => new { e.CisId, e.BranchCode })
            .HasDatabaseName("IX_LoanApplications_CisId_BranchCode");
    }
}