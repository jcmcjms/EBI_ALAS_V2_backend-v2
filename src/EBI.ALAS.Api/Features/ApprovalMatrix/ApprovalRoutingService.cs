namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public sealed class ApprovalRoutingService : IApprovalRoutingService
{
    public Task<ApprovalRoutingResult> ResolveApproverAsync(int loanApplicationId, CancellationToken ct = default)
    {
        return Task.FromResult(new ApprovalRoutingResult(null, 1, "Approver", null, null, null));
    }

    public Task<IReadOnlyList<ApprovalAuthority>> GetAuthoritiesAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<ApprovalAuthority>>([]);
    }
}