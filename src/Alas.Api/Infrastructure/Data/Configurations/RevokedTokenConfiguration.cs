using Alas.Api.Features.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.TokenId)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(e => e.TokenId)
            .IsUnique();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.RevokedAt)
            .IsRequired();
    }
}