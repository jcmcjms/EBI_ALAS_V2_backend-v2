namespace EBI.ALAS.Api.Features.Account;

public sealed record AccountProfileDto(
    int Id,
    string Username,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string Role,
    string BranchCode,
    string? JobTitle,
    string? ESignature,
    bool MustChangePassword);

public sealed record UpdateProfileRequest(
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string? JobTitle,
    string? ESignature);

public sealed record SessionDto(
    int Id,
    string IpAddress,
    string UserAgent,
    DateTime CreatedAt,
    DateTime? LastActivityAt,
    bool IsCurrent);

public sealed record ActivityItemDto(
    DateTime Timestamp,
    string Action,
    string EntityType,
    int EntityId,
    string Details);

public sealed record ProcessedLoanDto(
    int Id,
    string LamId,
    string ClientName,
    string Product,
    decimal Amount,
    string Status,
    DateTime ProcessedAt);

public sealed record RecentClientDto(
    string CisId,
    string Name,
    int LoanCount,
    DateTime LastTransaction);