using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.ApprovalMatrix;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class DeviationCatalogItemConfiguration : IEntityTypeConfiguration<DeviationCatalogItem>
{
    public void Configure(EntityTypeBuilder<DeviationCatalogItem> builder)
    {
        builder.ToTable("DeviationCatalog");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Code).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(200).IsRequired();
        builder.HasIndex(d => d.Code).IsUnique();
    }
}