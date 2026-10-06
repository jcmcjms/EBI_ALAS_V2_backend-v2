namespace Alas.Api.Features.Users;

public record UserQueryParameters(
    string? Search,
    string? Role,
    string? BranchId,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 20
);

public record CreateUserRequest(
    string Username,
    string Password,
    string FirstName,
    string? MiddleName,
    string LastName,
    string BranchId,
    string Role,
    string? JobTitle = null,
    string? Email = null,
    string? Phone = null
);

public record UpdateUserRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string BranchId,
    string Role,
    string? JobTitle = null,
    string? Email = null,
    string? Phone = null
);

public record UserStatusRequest(bool IsActive);

public record ResetPasswordRequest(string? NewPassword = null);

public sealed record ResetPasswordResponse(string Username, string TemporaryPassword, bool MustChangePassword);

public record UserResponse(
    int Id,
    string Username,
    string FirstName,
    string? MiddleName,
    string LastName,
    string BranchId,
    string Role,
    bool IsActive,
    DateTimeOffset CreatedAt,
    string? JobTitle = null,
    string? Email = null,
    string? Phone = null
);