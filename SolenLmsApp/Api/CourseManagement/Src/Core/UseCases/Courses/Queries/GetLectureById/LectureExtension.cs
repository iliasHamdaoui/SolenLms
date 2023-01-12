using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;

internal static class LectureExtension
{
    public static GetLectureByIdQueryResult ToQueryResult(this Lecture lecture)
    {
        return new GetLectureByIdQueryResult
        {
            Title = lecture.Title,
            Duration = lecture.Duration,
            Order = lecture.Order,
            ResourceId = lecture.ResourceId,
            MediaType = lecture.Type?.MediaType?.Value,
            Type = lecture.Type?.Value
        };
    }
}