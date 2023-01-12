using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses;

internal static class CourseExtension
{
    public static CoursesListItem ToCourseItem(this Course course, IHashids hashids)
    {
        return new CoursesListItem
        {
            Id = hashids.Encode(course.Id),
            Title = course.Title,
            Description = course.Description,
            InstructorName = course.Instructor?.FullName,
            IsPublished = course.IsPublished,
            LastUpdate = course.LastModifiedAt,
            Duration = course.Modules.SelectMany(x => x.Lectures).Sum(x => x.Duration)
        };
    }
}