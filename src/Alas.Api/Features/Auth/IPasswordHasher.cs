namespace Alas.Api.Features.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password, int workFactor = 12);
    bool VerifyPassword(string password, string hashedPassword);

    public const int TemporaryWorkFactor = 4;
}