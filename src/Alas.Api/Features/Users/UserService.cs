using Alas.Api.Composition;
using Alas.Api.Features.Auth;
using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TimeProvider _timeProvider;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, TimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _timeProvider = timeProvider;
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters, CancellationToken ct = default) =>
        await _userRepository.GetUsersAsync(parameters, ct);

    public async Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetUserByIdAsync(id, ct);
        if (user is null) return null;
        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        if (await _userRepository.UsernameExistsAsync(request.Username, ct: ct))
            throw new InvalidOperationException("Username already exists");

        var now = _timeProvider.GetUtcNow();
        var user = new User
        {
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password, IPasswordHasher.TemporaryWorkFactor),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            BranchId = request.BranchId,
            Role = request.Role,
            IsActive = true,
            MustChangePassword = true,
            CreatedAt = now,
            TempPasswordExpiresAt = now.AddHours(24),
            JobTitle = request.JobTitle,
            Email = request.Email,
            Phone = request.Phone
        };

        await _userRepository.AddUserAsync(user, ct);
        return MapToResponse(user);
    }

    public async Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetUserByIdAsync(id, ct);
        if (user is null) return null;

        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;
        user.BranchId = request.BranchId;
        user.Role = request.Role;
        user.JobTitle = request.JobTitle;
        user.Email = request.Email;
        user.Phone = request.Phone;

        await _userRepository.UpdateUserAsync(ct);
        return MapToResponse(user);
    }

    public async Task<bool> UpdateUserStatusAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var user = await _userRepository.GetUserByIdAsync(id, ct);
        if (user is null) return false;

        user.IsActive = isActive;
        await _userRepository.UpdateUserAsync(ct);
        return true;
    }

    public async Task<bool> ForcePasswordResetAsync(int id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetUserByIdAsync(id, ct);
        if (user is null) return false;

        user.MustChangePassword = true;
        await _userRepository.UpdateUserAsync(ct);
        return true;
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(int id, string newPassword, CancellationToken ct = default)
    {
        var user = await _userRepository.GetUserByIdAsync(id, ct);
        if (user is null)
            throw new NotFoundException("User", id);

        user.PasswordHash = _passwordHasher.HashPassword(newPassword, IPasswordHasher.TemporaryWorkFactor);
        user.MustChangePassword = true;
        user.TempPasswordExpiresAt = _timeProvider.GetUtcNow().AddHours(24);
        await _userRepository.UpdateUserAsync(ct);

        return new ResetPasswordResponse(user.Username, newPassword, user.MustChangePassword);
    }

    private static UserResponse MapToResponse(User user) =>
        new(
            user.Id,
            user.Username,
            user.FirstName,
            user.MiddleName,
            user.LastName,
            user.BranchId,
            user.Role,
            user.IsActive,
            user.CreatedAt,
            user.JobTitle,
            user.Email,
            user.Phone);
}