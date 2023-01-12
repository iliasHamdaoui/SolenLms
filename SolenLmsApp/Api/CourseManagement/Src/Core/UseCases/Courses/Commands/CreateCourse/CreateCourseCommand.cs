using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateCourse;

public sealed record CreateCourseCommand : IRequest<RequestResponse<string>>
{
    public string CourseTitle { get; set; } = default!;
    public string? CourseDescription { get; set; }

}
