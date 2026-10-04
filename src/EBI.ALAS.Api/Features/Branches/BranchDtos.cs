namespace EBI.ALAS.Api.Features.Branches;

public sealed record BranchDto(
    int Id,
    string Code,
    string Name,
    string AreaCode,
    string Region,
    string Address,
    string? Phone,
    string? Email,
    bool IsActive);

public sealed record BranchSimpleDto(
    string Code,
    string Name);