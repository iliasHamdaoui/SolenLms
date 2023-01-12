using FluentValidation;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.UpdateOrganization;
public sealed class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationName).MaximumLength(60).NotEmpty();
    }
}
