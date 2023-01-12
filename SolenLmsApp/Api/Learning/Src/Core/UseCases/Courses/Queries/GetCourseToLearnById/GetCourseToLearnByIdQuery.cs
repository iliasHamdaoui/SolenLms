using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseToLearnById;
public sealed record GetCourseToLearnByIdQuery : IRequest<RequestResponse<GetCourseToLearnByIdQueryResult>>
{
    public string CourseId { get; }

    public GetCourseToLearnByIdQuery(string courseId)
    {
        CourseId = courseId;
    }
}
