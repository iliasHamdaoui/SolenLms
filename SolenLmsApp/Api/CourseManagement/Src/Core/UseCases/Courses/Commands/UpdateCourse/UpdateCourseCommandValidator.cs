namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateCourse;

public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.CourseTitle).NotEmpty().MaximumLength(60);
        RuleFor(x => x.CourseDescription).MaximumLength(200);
    }
}
