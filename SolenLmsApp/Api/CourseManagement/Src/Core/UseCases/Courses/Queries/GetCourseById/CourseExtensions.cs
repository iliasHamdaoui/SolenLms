using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;

internal static class CourseExtensions
{
    private static LectureForGetCourseByIdQueryResult ToLectureResult(this Lecture lecture, IHashids hashids)
    {
        return new LectureForGetCourseByIdQueryResult(hashids.Encode(lecture.Id), lecture.Title, lecture.Type.Value,
            lecture.Duration, lecture.Order, lecture.ResourceId);
    }

    private static ModuleForGetCourseByIdQueryResult ToModuleResult(this Module module, IHashids hashids)
    {
        List<LectureForGetCourseByIdQueryResult> lectures =
            module.Lectures.OrderBy(x => x.Order).Select(x => x.ToLectureResult(hashids)).ToList();

        return new ModuleForGetCourseByIdQueryResult
        {
            Id = hashids.Encode(module.Id),
            Title = module.Title,
            Duration = lectures.Sum(x => x.Duration),
            Order = module.Order,
            Lectures = lectures
        };
    }

    public static GetCourseByIdQueryResult ToQueryResult(this Course course, IHashids hashids)
    {
        List<ModuleForGetCourseByIdQueryResult> modules =
            course.Modules.OrderBy(x => x.Order).Select(x => x.ToModuleResult(hashids)).ToList();

        return new GetCourseByIdQueryResult
        {
            CourseId = hashids.Encode(course.Id),
            Title = course.Title,
            Description = course.Description,
            Duration = modules.Sum(x => x.Duration),
            IsPublished = course.IsPublished,
            PublicationDate = course.PublicationDate,
            InstructorId = course.InstructorId,
            InstructorName = course.Instructor?.FullName,
            CreatedAt = course.CreatedAt,
            LastModifiedAt = course.LastModifiedAt,
            Modules = modules
        };
    }
}