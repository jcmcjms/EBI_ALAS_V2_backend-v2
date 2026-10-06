using Alas.Api.Composition;
using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Users;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters, CancellationToken ct = default);
    Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken ct = default);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
    Task<bool> UpdateUserStatusAsync(int id, bool isActive, CancellationToken ct = default);
    Task<bool> ForcePasswordResetAsync(int id, CancellationToken ct = default);
    Task<ResetPasswordResponse> ResetPasswordAsync(int id, string newPassword, CancellationToken ct = default);
}