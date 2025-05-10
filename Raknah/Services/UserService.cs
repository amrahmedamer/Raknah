using Microsoft.AspNetCore.Identity;
using Raknah.Consts.Errors;
using Raknah.Contracts.User;

namespace Raknah.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;


    public async Task<Result<UserProfileResponse>> GetAsync(string id)
    {

        var user = await _userManager.FindByIdAsync(id);

        return Result.Success(user.Adapt<UserProfileResponse>());
    }

    public async Task<Result> UpdateProfileAsync(string id, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);

        user = request.Adapt(user);
        var result = await _userManager.UpdateAsync(user!);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(UserError.UserUpdateFailed);
    }
    public async Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(Id);
        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(UserError.PasswordChangeFailed);
    }

}
