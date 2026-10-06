using Alas.Api.Features.ApprovalMatrix.Domain;

namespace Alas.Api.Features.ApprovalMatrix;

public record ApprovalAuthorityResponse(
    string Key,
    string DisplayName,
    int Tier,
    int Priority,
    bool AllowNew,
    bool AllowRenewal,
    DeviationSeverity MaxSeverity,
    decimal MaxTotalExposure,
    AuthorityScope ScopeType);

public record CreateApprovalAuthorityRequest(
    string Key,
    string DisplayName,
    int Tier,
    int Priority,
    bool AllowNew,
    bool AllowRenewal,
    DeviationSeverity MaxSeverity,
    decimal MaxTotalExposure,
    AuthorityScope ScopeType);

public record UpdateApprovalAuthorityRequest(
    string DisplayName,
    int Tier,
    int Priority,
    bool AllowNew,
    bool AllowRenewal,
    DeviationSeverity MaxSeverity,
    decimal MaxTotalExposure,
    AuthorityScope ScopeType);

public record DeviationCatalogResponse(
    int Id,
    string Description,
    DeviationSeverity Severity);

public record CreateDeviationCatalogRequest(
    string Description,
    DeviationSeverity Severity);

public record UpdateDeviationCatalogRequest(
    string Description,
    DeviationSeverity Severity);