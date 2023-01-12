using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseToLearnById;

internal static class CourseExtension
{
    public static GetCourseToLearnByIdQueryResult ToQueryResult(this Course course,
        LearnerCourseAccess? lastAccessedLecture)
    {
        IEnumerable<ModuleForGetCourseToLearnByIdQueryResult> modules = GetCourseModules(course);
        
        return new GetCourseToLearnByIdQueryResult
        {
            CourseId = course.Id,
            Title = course.Title,
            Duration = course.Duration,
            FirstLecture = lastAccessedLecture?.LectureId ?? modules.FirstOrDefault()?.Lectures.FirstOrDefault()?.Id,
            LearnerProgress = course.LearnersProgress.FirstOrDefault()?.Progress ?? 0,
            Modules = modules
        };
    }

    private static List<ModuleForGetCourseToLearnByIdQueryResult> GetCourseModules(Course course)
    {
        List<ModuleForGetCourseToLearnByIdQueryResult> modules = new();
        LectureForGetCourseToLearnByIdQueryResult? previousLecture = null;
        foreach (Module module in course.Modules.OrderBy(x => x.Order))
        {
            List<LectureForGetCourseToLearnByIdQueryResult> lectures = new();
            foreach (Lecture lecture in module.Lectures.OrderBy(x => x.Order))
            {
                LectureForGetCourseToLearnByIdQueryResult lectureToReturn = new()
                {
                    Id = lecture.Id,
                    Title = lecture.Title,
                    LectureType = lecture.Type.Value,
                    Order = lecture.Order,
                    Duration = lecture.Duration,
                    ResourceId = lecture.ResourceId,
                };

                if (previousLecture is not null)
                {
                    previousLecture.NextLectureId = lectureToReturn.Id;
                    lectureToReturn.PreviousLectureId = previousLecture.Id;
                }

                lectures.Add(lectureToReturn);
                previousLecture = lectureToReturn;
            }

            modules.Add(new ModuleForGetCourseToLearnByIdQueryResult
            {
                Id = module.Id,
                Title = module.Title,
                Duration = module.Duration,
                Order = module.Order,
                Lectures = lectures
            });
        }

        return modules;
    }
}