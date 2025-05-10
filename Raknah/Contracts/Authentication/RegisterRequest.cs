namespace Raknah.Contracts.Authentication;

public record RegisterRequest(
    string FullName,
    string PhoneNumber,
    string Email,
    string Password
);