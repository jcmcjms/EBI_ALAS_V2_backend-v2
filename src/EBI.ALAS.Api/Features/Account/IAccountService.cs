using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Account;

public interface IAccountRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}

public interface IAccountService
{
    Task<AccountProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default);
    Task<AccountProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<SessionDto>> GetSessionsAsync(int userId, CancellationToken ct = default);
    Task<IReadOnlyList<ActivityItemDto>> GetActivityAsync(int userId, int limit, CancellationToken ct = default);
    Task<IReadOnlyList<ProcessedLoanDto>> GetProcessedLoansAsync(int userId, int limit, CancellationToken ct = default);
    Task<IReadOnlyList<RecentClientDto>> GetRecentClientsAsync(int userId, int limit, CancellationToken ct = default);
}