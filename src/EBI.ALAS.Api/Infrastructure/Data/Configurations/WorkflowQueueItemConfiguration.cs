using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EBI.ALAS.Api.Features.Loans;

namespace EBI.ALAS.Api.Infrastructure.Data.Configurations;

public sealed class WorkflowQueueItemConfiguration : IEntityTypeConfiguration<WorkflowQueueItem>
{
    public void Configure(EntityTypeBuilder<WorkflowQueueItem> builder)
    {
        builder.ToTable("WorkflowQueueItems");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.QueuePartition).HasMaxLength(100).IsRequired();
        builder.Property(w => w.Stage).HasMaxLength(50).IsRequired();
        builder.Property(w => w.State).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasIndex(w => new { w.QueuePartition, w.State, w.Position });
        builder.HasIndex(w => w.LoanApplicationId);
        builder.HasIndex(w => w.OwnerUserId);
    }
}