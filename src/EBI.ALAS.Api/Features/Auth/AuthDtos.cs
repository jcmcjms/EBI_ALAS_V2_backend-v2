namespace EBI.ALAS.Api.Features.Auth;

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, UserDto User);

public sealed record RefreshRequest;

public sealed record RefreshResponse(string AccessToken, DateTime ExpiresAt);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public sealed record UserDto(
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
    bool MustChangePassword);