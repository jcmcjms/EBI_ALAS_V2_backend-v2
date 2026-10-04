using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.ToTable("LoanApplications");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.LamId).HasMaxLength(50).IsRequired();
        builder.Property(l => l.ApplicationGroupNo).HasMaxLength(50).IsRequired();
        builder.Property(l => l.BranchCode).HasMaxLength(10).IsRequired();
        builder.Property(l => l.CreationTypeLabel).HasMaxLength(100);
        builder.Property(l => l.RequestingOfficer).HasMaxLength(100);
        builder.Property(l => l.Lai).HasMaxLength(100);
        builder.Property(l => l.CisId).HasMaxLength(50);
        builder.Property(l => l.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(l => l.MiddleName).HasMaxLength(50);
        builder.Property(l => l.LastName).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Suffix).HasMaxLength(20);
        builder.Property(l => l.Address).HasMaxLength(255);
        builder.Property(l => l.Agency).HasMaxLength(100);
        builder.Property(l => l.Position).HasMaxLength(100);
        builder.Property(l => l.EmployeeId).HasMaxLength(50);
        builder.Property(l => l.LengthOfService).HasMaxLength(50);
        builder.Property(l => l.Region).HasMaxLength(50);
        builder.Property(l => l.DivisionCode).HasMaxLength(20);
        builder.Property(l => l.StationCode).HasMaxLength(20);
        builder.Property(l => l.MisAgency).HasMaxLength(100);
        builder.Property(l => l.School).HasMaxLength(200);
        builder.Property(l => l.Referrer).HasMaxLength(100);
        builder.Property(l => l.LoanNo).HasMaxLength(50);
        builder.Property(l => l.ProductCode).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Product).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Purpose).HasMaxLength(200);
        builder.Property(l => l.VerificationFindings).HasMaxLength(1000);
        builder.Property(l => l.Remarks).HasMaxLength(1000);
        builder.Property(l => l.AoRecommendation).HasMaxLength(500);
        builder.Property(l => l.OtherRemarks).HasMaxLength(500);
        builder.Property(l => l.FeeDeviationJustification).HasMaxLength(500);
        builder.Property(l => l.Status).HasMaxLength(50).IsRequired().HasDefaultValue("Draft");
        builder.Property(l => l.LoanType).HasMaxLength(20).HasDefaultValue("New");
        builder.Property(l => l.NoAuthorityReason).HasMaxLength(500);
        builder.Property(l => l.DocumentFlagReason).HasMaxLength(500);
        builder.Property(l => l.WebLoanCisNo).HasMaxLength(50);
        builder.Property(l => l.WebLoanBranchCode).HasMaxLength(10);
        builder.Property(l => l.PreLoanFormNumber).HasMaxLength(50);
        builder.HasIndex(l => l.LamId).IsUnique();
        builder.HasIndex(l => l.ApplicationGroupNo);
        builder.HasIndex(l => l.BranchCode);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.CreatedById);
        builder.HasIndex(l => l.AssignedApproverId);
        builder.HasIndex(l => l.ApplicationDate);
        builder.HasIndex(l => l.LastActionDate);

        builder.HasOne(l => l.CreatedBy)
            .WithMany()
            .HasForeignKey(l => l.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.AssignedApprover)
            .WithMany()
            .HasForeignKey(l => l.AssignedApproverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(l => l.DocumentsFlaggedBy)
            .WithMany()
            .HasForeignKey(l => l.DocumentsFlaggedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}