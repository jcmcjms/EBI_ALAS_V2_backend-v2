using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class LoanProductConfiguration : IEntityTypeConfiguration<LoanProduct>
{
    public void Configure(EntityTypeBuilder<LoanProduct> builder)
    {
        builder.ToTable("LoanProducts");
        builder.HasKey(l => l.ProductCode);
        builder.Property(l => l.ProductCode).HasMaxLength(20).IsRequired();
        builder.Property(l => l.ProductName).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(500);
    }
}