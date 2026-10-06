namespace Alas.Api.Features.Account;

public interface IAccountRepository
{
    Task<AccountProfileResponse?> GetProfileAsync(int userId, CancellationToken ct = default);
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task<PagedSessionsResponse> GetActiveSessionsAsync(int userId, int? currentSessionId, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<SessionRevokeResult> RevokeSessionAsync(int userId, int sessionId, int? currentSessionId, CancellationToken ct = default);
    Task<int> RevokeOtherSessionsAsync(int userId, int? currentSessionId, CancellationToken ct = default);
    Task<List<ActivityResponse>> GetRecentActivityAsync(int userId, int limit = 10, CancellationToken ct = default);
    Task<List<ProcessedLoanResponse>> GetProcessedLoansAsync(int userId, int limit = 10, CancellationToken ct = default);
    Task<List<RecentClientResponse>> GetRecentClientsAsync(int userId, int limit = 5, CancellationToken ct = default);
}