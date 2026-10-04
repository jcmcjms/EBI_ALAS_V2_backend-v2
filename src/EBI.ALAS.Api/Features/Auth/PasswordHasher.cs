namespace EBI.ALAS.Api.Features.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: BCrypt.Net.HashType.SHA384);

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
        catch
        {
            // Fallback for legacy hashes
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}