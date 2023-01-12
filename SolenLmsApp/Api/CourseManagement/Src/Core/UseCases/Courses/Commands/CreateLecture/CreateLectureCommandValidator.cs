namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateLecture;

public sealed class CreateLectureCommandValidator : AbstractValidator<CreateLectureCommand>
{
    public CreateLectureCommandValidator()
    {
        RuleFor(x => x.LectureTitle).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LectureType).NotEmpty();
    }
}
