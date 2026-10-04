using EBI.ALAS.Api.Features.Auth;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Users;

public interface IUserService
{
    Task<Result<UserDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<UserListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? role, string? branchCode, bool? isActive, CancellationToken ct = default);
    Task<Result<UserDetailDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<UserDetailDto>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
    Task<Result> ChangeStatusAsync(int id, ChangeUserStatusRequest request, CancellationToken ct = default);
}

public sealed class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITimeProvider timeProvider) : IUserService
{
    public async Task<Result<UserDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct);
        if (user is null)
        {
            return Result<UserDetailDto>.Failure("USER_NOT_FOUND", "User not found");
        }
        return Result<UserDetailDto>.Success(MapToDetailDto(user));
    }

    public Task<PagedResult<UserListItemDto>> GetPagedAsync(
        int page, int pageSize, string? search, string? role, string? branchCode, bool? isActive, CancellationToken ct = default) =>
        userRepository.GetPagedAsync(page, pageSize, search, role, branchCode, isActive, ct);

    public async Task<Result<UserDetailDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        if (await userRepository.UsernameExistsAsync(request.Username, ct: ct))
        {
            return Result<UserDetailDto>.Failure("USERNAME_EXISTS", "Username already exists");
        }

        if (await userRepository.EmailExistsAsync(request.Email, ct: ct))
        {
            return Result<UserDetailDto>.Failure("EMAIL_EXISTS", "Email already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Suffix = request.Suffix,
            Role = request.Role,
            BranchCode = request.BranchCode,
            JobTitle = request.JobTitle,
            CreatedAt = timeProvider.UtcNow,
            MustChangePassword = true
        };

        await userRepository.CreateAsync(user, ct);
        return Result<UserDetailDto>.Success(MapToDetailDto(user));
    }

    public async Task<Result<UserDetailDto>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct);
        if (user is null)
        {
            return Result<UserDetailDto>.Failure("USER_NOT_FOUND", "User not found");
        }

        if (await userRepository.EmailExistsAsync(request.Email, id, ct))
        {
            return Result<UserDetailDto>.Failure("EMAIL_EXISTS", "Email already exists");
        }

        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;
        user.Suffix = request.Suffix;
        user.Role = request.Role;
        user.BranchCode = request.BranchCode;
        user.JobTitle = request.JobTitle;
        user.ESignature = request.ESignature;
        user.IsActive = request.IsActive;

        await userRepository.UpdateAsync(user, ct);
        return Result<UserDetailDto>.Success(MapToDetailDto(user));
    }

    public async Task<Result> ChangeStatusAsync(int id, ChangeUserStatusRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct);
        if (user is null)
        {
            return Result.Failure("USER_NOT_FOUND", "User not found");
        }

        user.IsActive = request.IsActive;
        await userRepository.UpdateAsync(user, ct);
        return Result.Success();
    }

    private static UserDetailDto MapToDetailDto(User user) => new(
        user.Id, user.Username, user.Email, user.FirstName, user.MiddleName, user.LastName, user.Suffix,
        user.Role, user.BranchCode, user.JobTitle, user.ESignature, user.IsActive, user.MustChangePassword,
        user.CreatedAt, user.LastLoginAt, user.PasswordChangedAt);
}