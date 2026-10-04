using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Account;

public sealed class AccountService(
    IAccountRepository accountRepository) : IAccountService
{
    public async Task<AccountProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await accountRepository.GetByIdAsync(userId, ct);
        if (user is null) return null;

        return new AccountProfileDto(
            user.Id, user.Username, user.Email, user.FirstName, user.MiddleName, user.LastName, user.Suffix,
            user.Role, user.BranchCode, user.JobTitle, user.ESignature, user.MustChangePassword);
    }

    public async Task<AccountProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await accountRepository.GetByIdAsync(userId, ct);
        if (user is null) throw new InvalidOperationException("User not found");

        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;
        user.Suffix = request.Suffix;
        user.JobTitle = request.JobTitle;
        user.ESignature = request.ESignature;

        await accountRepository.UpdateAsync(user, ct);

        return new AccountProfileDto(
            user.Id, user.Username, user.Email, user.FirstName, user.MiddleName, user.LastName, user.Suffix,
            user.Role, user.BranchCode, user.JobTitle, user.ESignature, user.MustChangePassword);
    }

    public Task<IReadOnlyList<SessionDto>> GetSessionsAsync(int userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SessionDto>>([]);

    public Task<IReadOnlyList<ActivityItemDto>> GetActivityAsync(int userId, int limit, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ActivityItemDto>>([]);

    public Task<IReadOnlyList<ProcessedLoanDto>> GetProcessedLoansAsync(int userId, int limit, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ProcessedLoanDto>>([]);

    public Task<IReadOnlyList<RecentClientDto>> GetRecentClientsAsync(int userId, int limit, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<RecentClientDto>>([]);
}