using Alas.Api.Features.ApprovalMatrix.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class ApprovalAuthorityConfiguration : IEntityTypeConfiguration<ApprovalAuthority>
{
    public void Configure(EntityTypeBuilder<ApprovalAuthority> builder)
    {
        builder.HasKey(e => e.Key);

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Tier)
            .IsRequired();

        builder.Property(e => e.Priority)
            .IsRequired();

        builder.Property(e => e.AllowNew)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.AllowRenewal)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.MaxSeverity)
            .IsRequired();

        builder.Property(e => e.MaxTotalExposure)
            .HasPrecision(18, 2);

        builder.Property(e => e.ScopeType)
            .IsRequired();

        builder.HasIndex(e => new { e.Tier, e.Priority })
            .HasDatabaseName("IX_ApprovalAuthorities_Tier_Priority");
    }
}