using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.ApprovalMatrix;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class ApprovalAuthorityConfiguration : IEntityTypeConfiguration<ApprovalAuthority>
{
    public void Configure(EntityTypeBuilder<ApprovalAuthority> builder)
    {
        builder.ToTable("ApprovalAuthorities");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.LoanType).HasMaxLength(20).IsRequired();
        builder.Property(a => a.ApproverRole).HasMaxLength(50).IsRequired();
        builder.Property(a => a.BranchCode).HasMaxLength(10);
        builder.Property(a => a.AreaCode).HasMaxLength(10);
        builder.HasIndex(a => new { a.LoanType, a.MinExposure, a.MaxExposure, a.DeviationSeverity, a.Tier });
    }
}