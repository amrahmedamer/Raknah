using Raknah.Contracts.User;

namespace Raknah.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetAsync(string Id);
    Task<Result> UpdateProfileAsync(string Id, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request);
}
