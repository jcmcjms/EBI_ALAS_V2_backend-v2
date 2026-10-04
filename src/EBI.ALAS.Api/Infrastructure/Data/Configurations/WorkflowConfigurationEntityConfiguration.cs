using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class WorkflowConfigurationEntityConfiguration : IEntityTypeConfiguration<WorkflowConfiguration>
{
    public void Configure(EntityTypeBuilder<WorkflowConfiguration> builder)
    {
        builder.ToTable("WorkflowConfigurations");
        builder.HasKey(w => w.Id);
    }
}