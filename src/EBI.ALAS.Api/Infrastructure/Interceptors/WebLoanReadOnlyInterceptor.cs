using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EBI.ALAS.Api.Infrastructure.Interceptors;

public sealed class WebLoanReadOnlyInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        throw new InvalidOperationException("WebLoanDbContext is read-only. Write operations are not permitted.");
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("WebLoanDbContext is read-only. Write operations are not permitted.");
    }
}