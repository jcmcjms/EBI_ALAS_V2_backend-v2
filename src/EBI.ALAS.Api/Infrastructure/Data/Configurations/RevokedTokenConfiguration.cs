using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("RevokedTokens");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Jti).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Reason).HasMaxLength(200);
        builder.HasIndex(r => r.Jti).IsUnique();
        builder.HasIndex(r => r.ExpiresAt);
    }
}