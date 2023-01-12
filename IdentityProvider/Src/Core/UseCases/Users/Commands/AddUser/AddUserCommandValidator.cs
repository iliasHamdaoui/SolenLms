using FluentValidation;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.AddUser;
public sealed class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.Email).MaximumLength(60).EmailAddress().NotEmpty();
        RuleFor(x => x.FamilyName).MaximumLength(60);
        RuleFor(x => x.GivenName).MaximumLength(60);
        RuleFor(x => x.Roles).NotNull();
    }
}
