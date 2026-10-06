using System.ComponentModel.DataAnnotations;
using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Auth.Domain;

public sealed class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset AbsoluteExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? DeviceInfo { get; set; }
    public User User { get; set; } = null!;
}

public sealed class RevokedToken
{
    public int Id { get; set; }
    public string TokenId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset RevokedAt { get; set; }
}

public sealed class JwtSettings
{
    [Required]
    public string Secret { get; init; } = string.Empty;
    [Required]
    public string Issuer { get; init; } = string.Empty;
    [Required]
    public string Audience { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }
    [Range(1, int.MaxValue)]
    public int RefreshTokenExpiryDays { get; init; }
    [Range(1, int.MaxValue)]
    public int AbsoluteSessionExpiryDays { get; init; }
}