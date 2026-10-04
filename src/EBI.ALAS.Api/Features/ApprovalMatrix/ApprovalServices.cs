namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public interface IDocumentCompletenessService
{
    Task<bool> IsCompleteAsync(int loanApplicationId, CancellationToken ct = default);
    Task SyncAsync(int loanApplicationId, CancellationToken ct = default);
}

public sealed class DocumentCompletenessService : IDocumentCompletenessService
{
    public Task<bool> IsCompleteAsync(int loanApplicationId, CancellationToken ct = default) => Task.FromResult(true);
    public Task SyncAsync(int loanApplicationId, CancellationToken ct = default) => Task.CompletedTask;
}

public interface ILoanAssignmentService
{
    Task<AssignmentResult> AssignAsync(int loanApplicationId, int approverId, CancellationToken ct = default);
    Task ReleaseAsync(int loanApplicationId, int approverId, CancellationToken ct = default);
}

public sealed record AssignmentResult(bool Success, string? Error);

public sealed class LoanAssignmentService : ILoanAssignmentService
{
    public Task<AssignmentResult> AssignAsync(int loanApplicationId, int approverId, CancellationToken ct = default) =>
        Task.FromResult(new AssignmentResult(true, null));

    public Task ReleaseAsync(int loanApplicationId, int approverId, CancellationToken ct = default) =>
        Task.CompletedTask;
}