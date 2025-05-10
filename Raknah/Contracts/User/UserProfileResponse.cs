namespace Raknah.Contracts.User;

public record UserProfileResponse
(
    string FullName,
    string PhoneNumber,
    string Email,
    string UserName
 );
