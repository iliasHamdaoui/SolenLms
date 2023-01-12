using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;

internal static class ModuleExtensions
{
    private static LectureForGetModuleByIdQueryResult ToLectureResult(this Lecture lecture, IHashids hashids)
    {
        return new LectureForGetModuleByIdQueryResult
        {
            Id = hashids.Encode(lecture.Id),
            Duration = lecture.Duration,
            Title = lecture.Title,
            ResourceId = lecture.ResourceId,
            Type = lecture.Type.Value
        };
    }

    public static GetModuleByIdQueryResult ToQueryResult(this Module module, IHashids hashids)
    {
        List<LectureForGetModuleByIdQueryResult> lectures =
            module.Lectures.OrderBy(x => x.Order).Select(x => x.ToLectureResult(hashids)).ToList();


        return new GetModuleByIdQueryResult
        {
            Title = module.Title,
            Order = module.Order,
            Duration = lectures.Sum(x => x.Duration),
            Lectures = lectures
        };
    }
}