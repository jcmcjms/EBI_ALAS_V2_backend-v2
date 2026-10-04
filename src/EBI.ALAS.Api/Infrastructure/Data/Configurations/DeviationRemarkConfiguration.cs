using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class DeviationRemarkConfiguration : IEntityTypeConfiguration<DeviationRemark>
{
    public void Configure(EntityTypeBuilder<DeviationRemark> builder)
    {
        builder.ToTable("DeviationRemarks");
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.LoanDeviationId);
    }
}