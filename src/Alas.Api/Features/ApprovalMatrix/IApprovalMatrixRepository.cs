using Alas.Api.Features.ApprovalMatrix.Domain;

namespace Alas.Api.Features.ApprovalMatrix;

public interface IApprovalMatrixRepository
{
    // Approval Authorities
    Task<IReadOnlyList<ApprovalAuthority>> GetAllAuthoritiesAsync(CancellationToken ct = default);
    Task<ApprovalAuthority?> GetAuthorityByKeyAsync(string key, CancellationToken ct = default);
    Task<bool> AuthorityKeyExistsAsync(string key, CancellationToken ct = default);
    Task AddAuthorityAsync(ApprovalAuthority authority, CancellationToken ct = default);
    Task SaveAuthorityAsync(ApprovalAuthority authority, CancellationToken ct = default);
    Task<bool> DeleteAuthorityAsync(string key, CancellationToken ct = default);

    // Deviation Catalog
    Task<IReadOnlyList<DeviationCatalogItem>> GetAllDeviationCatalogAsync(CancellationToken ct = default);
    Task<DeviationCatalogItem?> GetDeviationCatalogByIdAsync(int id, CancellationToken ct = default);
    Task AddDeviationCatalogAsync(DeviationCatalogItem item, CancellationToken ct = default);
    Task SaveDeviationCatalogAsync(DeviationCatalogItem item, CancellationToken ct = default);
    Task<bool> DeleteDeviationCatalogAsync(int id, CancellationToken ct = default);
}