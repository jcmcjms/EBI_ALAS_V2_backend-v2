namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public sealed record ApprovalRoutingDto(
    int? MatchedAuthorityId,
    int Tier,
    string ApproverRole,
    string? AssignedApproverId,
    string? AssignedApproverName,
    bool IsDocumentComplete,
    string? NoAuthorityReason);

public sealed record ApproverDto(
    int UserId,
    string Name,
    string Role,
    string BranchCode,
    string AreaCode,
    bool IsOnline,
    int CurrentLoad,
    int MaxLoad);