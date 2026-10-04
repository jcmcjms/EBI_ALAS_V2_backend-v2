using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class DocumentRemarkConfiguration : IEntityTypeConfiguration<DocumentRemark>
{
    public void Configure(EntityTypeBuilder<DocumentRemark> builder)
    {
        builder.ToTable("DocumentRemarks");
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.LoanApplicationId);
        builder.HasIndex(d => d.DocumentChecklistId);
    }
}