using Alas.Api.Features.ApprovalMatrix.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class DeviationCatalogItemConfiguration : IEntityTypeConfiguration<DeviationCatalogItem>
{
    public void Configure(EntityTypeBuilder<DeviationCatalogItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Severity)
            .IsRequired();
    }
}