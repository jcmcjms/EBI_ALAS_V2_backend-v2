using Alas.Api.Features.ApprovalMatrix.Domain;

namespace Alas.Api.Features.ApprovalMatrix;

public interface IApprovalMatrixService
{
    // Approval Authorities
    Task<IReadOnlyList<ApprovalAuthorityResponse>> GetAllAuthoritiesAsync(CancellationToken ct = default);
    Task<ApprovalAuthorityResponse?> GetAuthorityByKeyAsync(string key, CancellationToken ct = default);
    Task<ApprovalAuthorityResponse> CreateAuthorityAsync(CreateApprovalAuthorityRequest request, CancellationToken ct = default);
    Task<ApprovalAuthorityResponse?> UpdateAuthorityAsync(string key, UpdateApprovalAuthorityRequest request, CancellationToken ct = default);
    Task<bool> DeleteAuthorityAsync(string key, CancellationToken ct = default);

    // Deviation Catalog
    Task<IReadOnlyList<DeviationCatalogResponse>> GetAllDeviationCatalogAsync(CancellationToken ct = default);
    Task<DeviationCatalogResponse?> GetDeviationCatalogByIdAsync(int id, CancellationToken ct = default);
    Task<DeviationCatalogResponse> CreateDeviationCatalogAsync(CreateDeviationCatalogRequest request, CancellationToken ct = default);
    Task<DeviationCatalogResponse?> UpdateDeviationCatalogAsync(int id, UpdateDeviationCatalogRequest request, CancellationToken ct = default);
    Task<bool> DeleteDeviationCatalogAsync(int id, CancellationToken ct = default);
}