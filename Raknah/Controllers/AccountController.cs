using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raknah.Contracts.User;
using System.Security.Claims;

namespace Raknah.Controllers;
[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var result = await _userService.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [HttpPost("info")]
    public async Task<IActionResult> Info(UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateProfileAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, request);
        return result.IsSuccess ? Ok() : result.ToProblem();


    }
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ChangePasswordAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, request);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}
