using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class EbiReloanConfiguration : IEntityTypeConfiguration<EbiReloan>
{
    public void Configure(EntityTypeBuilder<EbiReloan> builder)
    {
        builder.ToTable("EbiReloans");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.LoanApplicationId);
    }
}