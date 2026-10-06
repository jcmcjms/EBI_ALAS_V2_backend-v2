using Alas.Api.Features.Users.Domain;

namespace Alas.Api.Features.Auth;

public interface IJwtTokenService
{
    (string AccessToken, string XsrfToken) GenerateTokenWithXsrf(User user, int refreshTokenId);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}