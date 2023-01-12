namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateModule;

public sealed class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(x => x.ModuleTitle).NotEmpty().MaximumLength(60);
    }
}
