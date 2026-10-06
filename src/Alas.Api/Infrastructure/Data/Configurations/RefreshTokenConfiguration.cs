using Alas.Api.Features.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(e => e.TokenHash)
            .IsUnique();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.AbsoluteExpiresAt)
            .IsRequired();

        builder.Property(e => e.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.DeviceInfo)
            .HasMaxLength(500);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.UserId, e.IsRevoked, e.ExpiresAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_IsRevoked_ExpiresAt");
    }
}