namespace EBI.ALAS.Api.Features.Loans;

public interface IAuditLogger
{
    Task LogActionAsync(int loanApplicationId, int userId, string action, string? fromStatus, string toStatus, string? remarks, CancellationToken ct = default);
}

public sealed class AuditLogger : IAuditLogger
{
    public Task LogActionAsync(int loanApplicationId, int userId, string action, string? fromStatus, string toStatus, string? remarks, CancellationToken ct = default) =>
        Task.CompletedTask;
}