using Alas.Api.Composition;
using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Users;

public interface IUserRepository
{
    Task<PagedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters, CancellationToken ct = default);
    Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken ct = default);
    Task<bool> UsernameExistsAsync(string username, int? excludeId = null, CancellationToken ct = default);
    Task AddUserAsync(User user, CancellationToken ct = default);
    Task UpdateUserAsync(CancellationToken ct = default);
}