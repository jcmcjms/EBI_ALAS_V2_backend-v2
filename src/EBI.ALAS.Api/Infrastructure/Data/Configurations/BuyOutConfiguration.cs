using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class BuyOutConfiguration : IEntityTypeConfiguration<BuyOut>
{
    public void Configure(EntityTypeBuilder<BuyOut> builder)
    {
        builder.ToTable("BuyOuts");
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.LoanApplicationId);
    }
}