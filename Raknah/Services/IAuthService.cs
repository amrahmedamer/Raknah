using Raknah.Contracts.Authentication;

namespace Raknah.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> GenerateRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}
