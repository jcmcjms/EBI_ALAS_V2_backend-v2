namespace Alas.Api.Features.Account;

public record AccountProfileResponse(
    int Id,
    string Username,
    string FirstName,
    string? MiddleName,
    string LastName,
    string BranchId,
    string Role,
    string? Email,
    string? Phone,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PasswordChangedAt,
    AccountStatsResponse Stats);

public record AccountStatsResponse(
    int ProcessedLoans,
    int PendingLoans,
    int ApprovalRate);

public record UpdateProfileRequest(
    string? Email,
    string? Phone);

public record SessionResponse(
    int Id,
    string DeviceInfo,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrent);

public record PagedSessionsResponse(
    IReadOnlyList<SessionResponse> Items,
    int CurrentPage,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public record ActivityResponse(
    int Id,
    string LamId,
    string Action,
    string? FromStatus,
    string? ToStatus,
    string? Comments,
    DateTimeOffset ActionDate,
    string LoanClientName);

public record ProcessedLoanResponse(
    int Id,
    string LamId,
    string ClientName,
    string Status,
    DateTimeOffset ApplicationDate,
    decimal ProposedAmount);

public record RecentClientResponse(
    string CisId,
    string Name,
    string? Agency,
    DateTimeOffset LastInteraction);

public record RevokedSessionsResponse(int RevokedCount);

public enum SessionRevokeResult
{
    Revoked,
    NotFound,
    CurrentSession
}