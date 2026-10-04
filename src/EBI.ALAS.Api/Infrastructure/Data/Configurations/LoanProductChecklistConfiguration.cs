using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class LoanProductChecklistConfiguration : IEntityTypeConfiguration<LoanProductChecklist>
{
    public void Configure(EntityTypeBuilder<LoanProductChecklist> builder)
    {
        builder.ToTable("LoanProductChecklists");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.ProductCode).HasMaxLength(20).IsRequired();
        builder.Property(l => l.DocumentCode).HasMaxLength(50).IsRequired();
        builder.Property(l => l.DocumentName).HasMaxLength(100).IsRequired();
        builder.HasIndex(l => new { l.ProductCode, l.DocumentCode }).IsUnique();
    }
}