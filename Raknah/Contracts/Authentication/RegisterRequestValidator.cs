using FluentValidation;
using Raknah.Consts.Errors;

namespace Raknah.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^(?:\+20|0)?1[0125]\d{8}$")
            .WithMessage(UserError.PhoneNumberPattern);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(UserError.EmailIsNotValid);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
            .WithMessage(UserError.PasswordPattern);

    }
}
