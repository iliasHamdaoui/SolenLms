using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery : IRequest<RequestResponse<GetCourseByIdQueryResult>>
{
    public string CourseId { get; }

    public GetCourseByIdQuery(string courseId)
    {
        CourseId = courseId;
    }
}
