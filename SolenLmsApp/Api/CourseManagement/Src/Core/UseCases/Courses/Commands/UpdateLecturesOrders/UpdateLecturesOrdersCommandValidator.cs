namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecturesOrders;

public sealed class UpdateLecturesOrdersCommandValidator : AbstractValidator<UpdateLecturesOrdersCommand>
{
    public UpdateLecturesOrdersCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ModuleId).NotEmpty();
        RuleFor(x => x.LecturesOrders).NotNull();
    }
}