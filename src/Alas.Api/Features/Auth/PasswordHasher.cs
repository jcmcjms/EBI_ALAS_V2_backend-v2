namespace Alas.Api.Features.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password, int workFactor = 12) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor);

    public bool VerifyPassword(string password, string hashedPassword) =>
        BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}