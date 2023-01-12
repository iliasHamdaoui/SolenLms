namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.CourseTitle).NotEmpty().MaximumLength(60);
    }
}
