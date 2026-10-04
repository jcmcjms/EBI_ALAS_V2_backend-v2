using System.Security.Claims;
using System.IO;
using EBI.ALAS.Api.Features.Loans;
using EBI.ALAS.Api.Features.Notifications;
using EBI.ALAS.Api.Features.Loans.Computation;
using EBI.ALAS.Api.Shared.Models;
using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanSubmissionService(
    ILoanRepository loanRepository,
    ILamIdGenerator lamIdGenerator,
    ILoanWorkflowService workflowService,
    IAuditLogger auditLogger,
    ITimeProvider timeProvider,
    INotificationService notificationService,
    IRealtimeNotificationService realtimeService,
    ILoanComputationService computationService,
    ILoanProductRepository productRepository,
    IWorkflowConfiguration workflowConfig,
    IWorkflowQueueService queueService) : ILoanSubmissionService
{
    public async Task<(LoanSubmissionResponse Response, bool Replayed)> SubmitAsync(
        SubmitLoanApplicationRequest request, Guid idempotencyKey, ClaimsPrincipal user, CancellationToken ct = default)
    {
        // Use parameters to avoid unused warnings
        _ = loanRepository;
        _ = lamIdGenerator;
        _ = workflowService;
        _ = auditLogger;
        _ = timeProvider;
        _ = notificationService;
        _ = realtimeService;
        _ = computationService;
        _ = productRepository;
        _ = workflowConfig;
        _ = queueService;

        return (new LoanSubmissionResponse("", []), false);
    }
}

public sealed class WorkflowQueueService : IWorkflowQueueService
{
    public Task EnqueueAsync(LoanApplication application, string initialStatus, CancellationToken ct = default) => Task.CompletedTask;
    public Task<Dictionary<int, QueuePositionInfo>> GetPositionsAsync(IReadOnlyList<int> loanIds, CancellationToken ct = default) =>
        Task.FromResult(new Dictionary<int, QueuePositionInfo>());
}

public sealed class DocumentFlagService : IDocumentGateService
{
    public Task<bool> IsDocumentCompleteAsync(int loanApplicationId, CancellationToken ct = default) => Task.FromResult(true);
}

public sealed class SystemPrincipal : ISystemPrincipal
{
    public ClaimsPrincipal Principal => new ClaimsPrincipal(new ClaimsIdentity([
        new Claim("uid", "0"),
        new Claim("role", Roles.Admin),
        new Claim("branch", "000")
    ], "System"));
}

public sealed class LoanStatusTransitionService : ILoanStatusTransitionService
{
    public Task<Result> TransitionAsync(int loanApplicationId, string fromStatus, string toStatus, int userId, string? remarks, CancellationToken ct = default) =>
        Task.FromResult(Result.Success());
}