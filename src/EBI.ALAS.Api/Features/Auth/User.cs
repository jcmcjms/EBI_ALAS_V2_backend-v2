using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Auth;

public sealed class User
{
    public int Id { get; init; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public string Role { get; set; } = Roles.Encoder;
    public string BranchCode { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? ESignature { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
}