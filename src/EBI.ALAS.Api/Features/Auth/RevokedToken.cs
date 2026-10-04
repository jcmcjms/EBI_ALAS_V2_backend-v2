namespace EBI.ALAS.Api.Features.Auth;

public sealed class RevokedToken
{
    public int Id { get; init; }
    public string Jti { get; set; } = string.Empty;
    public int UserId { get; init; }
    public DateTime RevokedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string? Reason { get; set; }
}