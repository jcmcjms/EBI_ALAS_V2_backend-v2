namespace EBI.ALAS.Api.Features.Users;

public sealed record UserListItemDto(
    int Id,
    string Username,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string Role,
    string BranchCode,
    string? JobTitle,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

public sealed record UserDetailDto(
    int Id,
    string Username,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string Role,
    string BranchCode,
    string? JobTitle,
    string? ESignature,
    bool IsActive,
    bool MustChangePassword,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    DateTime? PasswordChangedAt);

public sealed record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string Role,
    string BranchCode,
    string? JobTitle);

public sealed record UpdateUserRequest(
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    string Role,
    string BranchCode,
    string? JobTitle,
    string? ESignature,
    bool IsActive);

public sealed record ChangeUserStatusRequest(bool IsActive);