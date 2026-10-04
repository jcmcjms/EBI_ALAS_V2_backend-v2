using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EBI.ALAS.Api.Infrastructure.Interceptors;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateTimestamps(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Entity is IHasCreatedAt createdAtEntity && entry.State == EntityState.Added)
            {
                createdAtEntity.CreatedAt = now;
            }

            if (entry.Entity is IHasModifiedAt modifiedAtEntity)
            {
                modifiedAtEntity.ModifiedAt = now;
            }
        }
    }
}

public interface IHasCreatedAt
{
    DateTime CreatedAt { get; set; }
}

public interface IHasModifiedAt
{
    DateTime ModifiedAt { get; set; }
}