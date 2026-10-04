using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class LoanDeviationConfiguration : IEntityTypeConfiguration<LoanDeviation>
{
    public void Configure(EntityTypeBuilder<LoanDeviation> builder)
    {
        builder.ToTable("LoanDeviations");
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.LoanApplicationId);
    }
}