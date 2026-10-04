using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.ApprovalMatrix;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class UserBranchCoverageConfiguration : IEntityTypeConfiguration<UserBranchCoverage>
{
    public void Configure(EntityTypeBuilder<UserBranchCoverage> builder)
    {
        builder.ToTable("UserBranchCoverages");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => new { u.UserId, u.BranchCode }).IsUnique();
    }
}