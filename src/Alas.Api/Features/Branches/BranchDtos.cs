namespace Alas.Api.Features.Branches;

public record BranchQueryParameters(
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 50
);

public record CreateBranchRequest(
    string Code,
    string Name,
    string? AreaCode = null);

public record UpdateBranchRequest(
    string Name,
    bool IsActive,
    string? AreaCode = null);

public record BranchResponse(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    DateTimeOffset CreatedAt,
    string? AreaCode = null);

public record BranchListResponse(
    int Id,
    string Code,
    string Name,
    bool IsActive);