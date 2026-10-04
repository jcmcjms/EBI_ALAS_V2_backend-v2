using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class DocumentChecklistConfiguration : IEntityTypeConfiguration<DocumentChecklist>
{
    public void Configure(EntityTypeBuilder<DocumentChecklist> builder)
    {
        builder.ToTable("DocumentChecklists");
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.LoanApplicationId);
    }
}