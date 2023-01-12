using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseById;

internal static class CourseExtension
{
    private static LectureForGetCourseByIdQueryResult ToLectureResult(this Lecture lecture)
    {
        return new LectureForGetCourseByIdQueryResult
        {
            Id = lecture.Id,
            Title = lecture.Title,
            LectureType = lecture.Type.Value,
            Order = lecture.Order,
            Duration = lecture.Duration
        };
    }

    private static ModuleForGetCourseByIdQueryResult ToModuleResult(this Module module)
    {
        return new ModuleForGetCourseByIdQueryResult
        {
            Id = module.Id,
            Title = module.Title,
            Duration = module.Duration,
            Order = module.Order,
            Lectures = module.Lectures.Select(x => x.ToLectureResult()).OrderBy(x => x.Order).ToList()
        };
    }

    public static GetCourseByIdQueryResult ToQueryResult(this Course course)
    {
        return new GetCourseByIdQueryResult
        {
            CourseId = course.Id,
            Title = course.Title,
            Description = course.Description,
            Duration = course.Duration,
            PublicationDate = course.PublicationDate,
            InstructorName = course.Instructor?.FullName,
            IsBookmarked = course.LearnersBookmarks.Any(),
            LearnerProgress = course.LearnersProgress.FirstOrDefault()?.Progress ?? 0,
            Categories = course.Categories.Select(x => x.Category.Name),
            Modules = course.Modules.Select(x => x.ToModuleResult()).OrderBy(x => x.Order).ToList()
        };
    }
}