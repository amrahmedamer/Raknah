namespace Raknah.Contracts.Authentication;

public record AuthResponse
(
    string Name,
    string Email,
    string Token,
    int ExpireIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration

);
