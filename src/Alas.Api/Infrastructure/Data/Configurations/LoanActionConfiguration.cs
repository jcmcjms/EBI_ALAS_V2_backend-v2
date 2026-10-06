using Alas.Api.Features.Loans.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alas.Api.Infrastructure.Data.Configurations;

public sealed class LoanActionConfiguration : IEntityTypeConfiguration<LoanAction>
{
    public void Configure(EntityTypeBuilder<LoanAction> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.FromStatus)
            .HasMaxLength(50);

        builder.Property(e => e.ToStatus)
            .HasMaxLength(50);

        builder.Property(e => e.Comments)
            .HasMaxLength(2000);

        builder.Property(e => e.ActionByRole)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(e => e.LoanApplication)
            .WithMany()
            .HasForeignKey(e => e.LoanApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.LoanApplicationId)
            .HasDatabaseName("IX_LoanActions_LoanApplicationId");
    }
}