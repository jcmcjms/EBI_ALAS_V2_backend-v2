using Alas.Api.Features.Loans.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class LoanProductConfiguration : IEntityTypeConfiguration<LoanProduct>
{
    public void Configure(EntityTypeBuilder<LoanProduct> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AmortizationMode)
            .IsRequired()
            .HasMaxLength(10);

        // Decimal precision for financial fields
        builder.Property(e => e.MinAmount).HasPrecision(18, 2);
        builder.Property(e => e.MaxAmount).HasPrecision(18, 2);
        builder.Property(e => e.NotarialFee).HasPrecision(18, 2);
        builder.Property(e => e.DocStampFee).HasPrecision(18, 2);
        builder.Property(e => e.InsuranceFee).HasPrecision(18, 2);
        builder.Property(e => e.AdvanceInterestRate).HasPrecision(10, 4);
        builder.Property(e => e.ApplicationChargeRate).HasPrecision(10, 4);

        builder.HasIndex(e => e.IsRetired)
            .HasDatabaseName("IX_LoanProducts_IsRetired");
    }
}