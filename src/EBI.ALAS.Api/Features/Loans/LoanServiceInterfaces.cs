using System.Security.Claims;
using EBI.ALAS.Api.Features.Loans;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Loans;

public interface ILoanSubmissionService
{
    Task<(LoanSubmissionResponse Response, bool Replayed)> SubmitAsync(
        SubmitLoanApplicationRequest request, Guid idempotencyKey, ClaimsPrincipal user, CancellationToken ct = default);
}

public interface IWorkflowQueueService
{
    Task EnqueueAsync(LoanApplication application, string initialStatus, CancellationToken ct = default);
    Task<Dictionary<int, QueuePositionInfo>> GetPositionsAsync(IReadOnlyList<int> loanIds, CancellationToken ct = default);
}

public interface IDocumentGateService
{
    Task<bool> IsDocumentCompleteAsync(int loanApplicationId, CancellationToken ct = default);
}

public interface ISystemPrincipal
{
    ClaimsPrincipal Principal { get; }
}

public interface ILoanStatusTransitionService
{
    Task<Result> TransitionAsync(int loanApplicationId, string fromStatus, string toStatus, int userId, string? remarks, CancellationToken ct = default);
}

public sealed record QueuePositionInfo(string Stage, int Position, int QueueLength, string? OwnerName, bool IsHead);