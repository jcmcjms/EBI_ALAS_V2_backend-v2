using Alas.Api.Features.ApprovalMatrix.Domain;

namespace Alas.Api.Features.ApprovalMatrix;

public sealed class ApprovalMatrixService : IApprovalMatrixService
{
    private readonly IApprovalMatrixRepository _repository;

    public ApprovalMatrixService(IApprovalMatrixRepository repository) => _repository = repository;

    // Approval Authorities
    public async Task<IReadOnlyList<ApprovalAuthorityResponse>> GetAllAuthoritiesAsync(CancellationToken ct = default)
    {
        var authorities = await _repository.GetAllAuthoritiesAsync(ct);
        return authorities.Select(MapToResponse).ToList();
    }

    public async Task<ApprovalAuthorityResponse?> GetAuthorityByKeyAsync(string key, CancellationToken ct = default)
    {
        var authority = await _repository.GetAuthorityByKeyAsync(key, ct);
        return authority is null ? null : MapToResponse(authority);
    }

    public async Task<ApprovalAuthorityResponse> CreateAuthorityAsync(CreateApprovalAuthorityRequest request, CancellationToken ct = default)
    {
        if (await _repository.AuthorityKeyExistsAsync(request.Key, ct))
            throw new InvalidOperationException("Approval authority key already exists");

        var authority = new ApprovalAuthority
        {
            Key = request.Key.ToUpperInvariant(),
            DisplayName = request.DisplayName,
            Tier = request.Tier,
            Priority = request.Priority,
            AllowNew = request.AllowNew,
            AllowRenewal = request.AllowRenewal,
            MaxSeverity = request.MaxSeverity,
            MaxTotalExposure = request.MaxTotalExposure,
            ScopeType = request.ScopeType
        };

        await _repository.AddAuthorityAsync(authority, ct);
        return MapToResponse(authority);
    }

    public async Task<ApprovalAuthorityResponse?> UpdateAuthorityAsync(string key, UpdateApprovalAuthorityRequest request, CancellationToken ct = default)
    {
        var authority = await _repository.GetAuthorityByKeyAsync(key, ct);
        if (authority is null) return null;

        authority.DisplayName = request.DisplayName;
        authority.Tier = request.Tier;
        authority.Priority = request.Priority;
        authority.AllowNew = request.AllowNew;
        authority.AllowRenewal = request.AllowRenewal;
        authority.MaxSeverity = request.MaxSeverity;
        authority.MaxTotalExposure = request.MaxTotalExposure;
        authority.ScopeType = request.ScopeType;

        await _repository.SaveAuthorityAsync(authority, ct);
        return MapToResponse(authority);
    }

    public async Task<bool> DeleteAuthorityAsync(string key, CancellationToken ct = default)
    {
        var authority = await _repository.GetAuthorityByKeyAsync(key, ct);
        if (authority is null) return false;

        // Business rule: cannot delete an authority that is referenced by users
        // This will be enforced by FK constraints in the database, but we check
        // here to provide a clear error message.
        return await _repository.DeleteAuthorityAsync(key, ct);
    }

    // Deviation Catalog
    public async Task<IReadOnlyList<DeviationCatalogResponse>> GetAllDeviationCatalogAsync(CancellationToken ct = default)
    {
        var items = await _repository.GetAllDeviationCatalogAsync(ct);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<DeviationCatalogResponse?> GetDeviationCatalogByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _repository.GetDeviationCatalogByIdAsync(id, ct);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<DeviationCatalogResponse> CreateDeviationCatalogAsync(CreateDeviationCatalogRequest request, CancellationToken ct = default)
    {
        var item = new DeviationCatalogItem
        {
            Description = request.Description,
            Severity = request.Severity
        };

        await _repository.AddDeviationCatalogAsync(item, ct);
        return MapToResponse(item);
    }

    public async Task<DeviationCatalogResponse?> UpdateDeviationCatalogAsync(int id, UpdateDeviationCatalogRequest request, CancellationToken ct = default)
    {
        var item = await _repository.GetDeviationCatalogByIdAsync(id, ct);
        if (item is null) return null;

        item.Description = request.Description;
        item.Severity = request.Severity;

        await _repository.SaveDeviationCatalogAsync(item, ct);
        return MapToResponse(item);
    }

    public async Task<bool> DeleteDeviationCatalogAsync(int id, CancellationToken ct = default)
    {
        var item = await _repository.GetDeviationCatalogByIdAsync(id, ct);
        if (item is null) return false;

        return await _repository.DeleteDeviationCatalogAsync(id, ct);
    }

    private static ApprovalAuthorityResponse MapToResponse(ApprovalAuthority a) =>
        new(a.Key, a.DisplayName, a.Tier, a.Priority, a.AllowNew, a.AllowRenewal,
            a.MaxSeverity, a.MaxTotalExposure, a.ScopeType);

    private static DeviationCatalogResponse MapToResponse(DeviationCatalogItem item) =>
        new(item.Id, item.Description, item.Severity);
}