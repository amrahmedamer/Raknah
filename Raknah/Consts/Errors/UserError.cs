namespace Raknah.Consts.Errors;

public static class UserError
{
    public static string EmailIsNotValid = "Email is not valid";
    public static string PasswordPattern = "Password must be at least 6 characters and contain one digit,uppercase letter,lowercase letter and special character";
    public static string PhoneNumberPattern = "Phone number is not valid";

    public static Error PasswordChangeFailed = new Error("PasswordChangeFailed", "Password change failed. Please ensure your current password is correct and meets the required criteria.", StatusCodes.Status400BadRequest);
    public static Error UserUpdateFailed = new Error("UserUpdateFailed", "User update failed. Please ensure all provided information is correct and try again.", StatusCodes.Status400BadRequest);
    public static Error UserRetrievalFailed = new Error("UserRetrievalFailed", "Failed to retrieve user information. Please ensure the user ID is correct and try again.", StatusCodes.Status404NotFound);
    public static Error InvalidCredentials = new Error("InvalidCredentials", "Invalid email or password. Please try again.", StatusCodes.Status401Unauthorized);
    public static Error EmailAlreadyExists = new Error("EmailAlreadyExists", "The email address is already in use. Please use a different email address.", StatusCodes.Status400BadRequest);
    public static Error UserCreationFailed = new Error("UserCreationFailed", "User creation failed. Please ensure all provided information is correct and try again.", StatusCodes.Status400BadRequest);
    public static Error UserNotFound = new Error("UserNotFound", "The user was not found. Please ensure the email address is correct and try again.", StatusCodes.Status404NotFound);
}
