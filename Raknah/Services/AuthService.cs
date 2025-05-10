using Microsoft.AspNetCore.Identity;
using Raknah.Authentications;
using Raknah.Consts.Errors;
using Raknah.Contracts.Authentication;
using System.Security.Cryptography;

namespace Raknah.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpires = 14;



    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        //check email and password
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Result.Failure<AuthResponse>(UserError.InvalidCredentials);

        //generate token
        (string token, int expiresIn) = _jwtProvider.GenerateJwtTokenAsync(user);

        //generate refresh token
        user.RefreshTokens.ToList().ForEach(token => token.RevokedOn = DateTime.Now);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpires);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expires = refreshTokenExpiration,
        });

        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthResponse(user.FullName, user.Email!, token, expiresIn, refreshToken, refreshTokenExpiration));
    }
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
        if (emailIsExist)
            return Result.Failure<AuthResponse>(UserError.EmailAlreadyExists);


        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Failure<AuthResponse>(UserError.UserCreationFailed);

        (string token, int expiresIn) = _jwtProvider.GenerateJwtTokenAsync(user);

        if (await _userManager.FindByEmailAsync(user.Email!) is not { } updateUser)
            return Result.Failure<AuthResponse>(UserError.UserNotFound);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpires);

        updateUser.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expires = refreshTokenExpiration
        });
        await _userManager.UpdateAsync(updateUser);
        return Result.Success(new AuthResponse(user.FullName, user.Email!, token, expiresIn, refreshToken, refreshTokenExpiration));

    }
    public async Task<AuthResponse?> GenerateRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return null;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
        if (userRefreshToken is null)
            return null;
        (string newToken, int newExpiresIn) = _jwtProvider.GenerateJwtTokenAsync(user);


        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpires);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            Expires = refreshTokenExpiration
        });
        await _userManager.UpdateAsync(user);
        return new AuthResponse(user.FullName, user.Email!, newToken, newExpiresIn, newRefreshToken, refreshTokenExpiration);


    }
    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
