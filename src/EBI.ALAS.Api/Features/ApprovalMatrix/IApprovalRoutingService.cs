namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public interface IApprovalRoutingService
{
    Task<ApprovalRoutingResult> ResolveApproverAsync(int loanApplicationId, CancellationToken ct = default);
    Task<IReadOnlyList<ApprovalAuthority>> GetAuthoritiesAsync(CancellationToken ct = default);
}

public sealed record ApprovalRoutingResult(
    int? MatchedAuthorityId,
    int Tier,
    string ApproverRole,
    string? AssignedApproverId,
    string? AssignedApproverName,
    string? NoAuthorityReason);