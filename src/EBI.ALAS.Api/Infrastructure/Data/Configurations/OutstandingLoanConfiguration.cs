using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class OutstandingLoanConfiguration : IEntityTypeConfiguration<OutstandingLoan>
{
    public void Configure(EntityTypeBuilder<OutstandingLoan> builder)
    {
        builder.ToTable("OutstandingLoans");
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.LoanApplicationId);
    }
}