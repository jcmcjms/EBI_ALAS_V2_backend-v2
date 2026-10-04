using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class LoanSubmissionIdempotencyConfiguration : IEntityTypeConfiguration<LoanSubmissionIdempotency>
{
    public void Configure(EntityTypeBuilder<LoanSubmissionIdempotency> builder)
    {
        builder.ToTable("LoanSubmissionIdempotencies");
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => new { i.IdempotencyKey, i.UserId }).IsUnique();
    }
}