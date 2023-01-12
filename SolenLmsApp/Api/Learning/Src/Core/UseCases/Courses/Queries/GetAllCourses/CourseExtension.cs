using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetAllCourses;

internal static class CourseExtension
{
    public static CoursesListItem ToListItem(this Course course)
    {
        return new CoursesListItem
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Duration = course.Duration,
            InstructorName = course.Instructor?.FullName,
            PublicationDate = course.PublicationDate,
            LastAccess = course.LearnersProgress?.FirstOrDefault()?.LastAccessTime,
            LearnerProgress = course.LearnersProgress?.FirstOrDefault()?.Progress ?? 0,
            IsBookmarked = course.LearnersBookmarks.Any(),
            Categories = course.Categories.Select(x => x.Category.Name)
        };
    }
}