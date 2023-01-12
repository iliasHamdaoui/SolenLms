namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModule;

public sealed class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
{
    public UpdateModuleCommandValidator()
    {
        RuleFor(x => x.ModuleTitle).NotEmpty().MaximumLength(60);
    }
}
