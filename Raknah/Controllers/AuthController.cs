using Microsoft.AspNetCore.Mvc;
using Raknah.Contracts.Authentication;

namespace Raknah.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Register(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GenerateRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result is null ? NotFound() : Ok(result);

    }
}
