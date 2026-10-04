using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Branches;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Code).HasMaxLength(10).IsRequired();
        builder.Property(b => b.Name).HasMaxLength(100).IsRequired();
        builder.Property(b => b.AreaCode).HasMaxLength(10).IsRequired();
        builder.Property(b => b.Region).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Address).HasMaxLength(255).IsRequired();
        builder.Property(b => b.Phone).HasMaxLength(50);
        builder.Property(b => b.Email).HasMaxLength(100);
        builder.HasIndex(b => b.Code).IsUnique();
    }
}