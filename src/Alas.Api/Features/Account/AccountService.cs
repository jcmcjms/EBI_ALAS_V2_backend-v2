namespace Alas.Api.Features.Account;

public sealed class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;

    public AccountService(IAccountRepository repository) => _repository = repository;

    public async Task<AccountProfileResponse?> GetProfileAsync(int userId, CancellationToken ct = default) =>
        await _repository.GetProfileAsync(userId, ct);

    public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        // Normalize inputs before persisting
        var normalized = request with
        {
            Email = Normalize(request.Email),
            Phone = Normalize(request.Phone)
        };

        return await _repository.UpdateProfileAsync(userId, normalized, ct);
    }

    public async Task<PagedSessionsResponse> GetActiveSessionsAsync(
        int userId, int? currentSessionId, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default) =>
        await _repository.GetActiveSessionsAsync(userId, currentSessionId, pageNumber, pageSize, ct);

    public async Task<SessionRevokeResult> RevokeSessionAsync(
        int userId, int sessionId, int? currentSessionId, CancellationToken ct = default) =>
        await _repository.RevokeSessionAsync(userId, sessionId, currentSessionId, ct);

    public async Task<int> RevokeOtherSessionsAsync(int userId, int? currentSessionId, CancellationToken ct = default) =>
        await _repository.RevokeOtherSessionsAsync(userId, currentSessionId, ct);

    public async Task<List<ActivityResponse>> GetRecentActivityAsync(int userId, int limit = 10, CancellationToken ct = default) =>
        await _repository.GetRecentActivityAsync(userId, limit, ct);

    public async Task<List<ProcessedLoanResponse>> GetProcessedLoansAsync(int userId, int limit = 10, CancellationToken ct = default) =>
        await _repository.GetProcessedLoansAsync(userId, limit, ct);

    public async Task<List<RecentClientResponse>> GetRecentClientsAsync(int userId, int limit = 5, CancellationToken ct = default) =>
        await _repository.GetRecentClientsAsync(userId, limit, ct);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}