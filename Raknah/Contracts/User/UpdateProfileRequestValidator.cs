using FluentValidation;
using Raknah.Consts.Errors;

namespace Raknah.Contracts.User;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^(?:\+20|0)?1[0125]\d{8}$")
            .WithMessage(UserError.PhoneNumberPattern);
    }
}
