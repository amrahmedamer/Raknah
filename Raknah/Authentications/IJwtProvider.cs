namespace Raknah.Authentications;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateJwtTokenAsync(ApplicationUser user);
    string? ValidateToken(string token);
}
