using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<PagedResult<UserListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? role, string? branchCode, bool? isActive, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task<bool> UsernameExistsAsync(string username, int? excludeId = null, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetByRoleAndBranchAsync(string role, string branchCode, CancellationToken ct = default);
}