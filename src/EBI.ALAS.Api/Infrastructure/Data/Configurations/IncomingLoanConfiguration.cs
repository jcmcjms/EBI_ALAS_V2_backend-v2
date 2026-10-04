using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class IncomingLoanConfiguration : IEntityTypeConfiguration<IncomingLoan>
{
    public void Configure(EntityTypeBuilder<IncomingLoan> builder)
    {
        builder.ToTable("IncomingLoans");
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.LoanApplicationId);
    }
}