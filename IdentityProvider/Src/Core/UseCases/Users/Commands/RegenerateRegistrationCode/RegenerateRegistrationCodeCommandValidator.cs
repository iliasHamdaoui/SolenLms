using FluentValidation;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.RegenerateRegistrationCode;
public sealed class RegenerateRegistrationCodeCommandValidator : AbstractValidator<RegenerateRegistrationCodeCommand>
{
    public RegenerateRegistrationCodeCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
    }
}
