using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.PublishCourse;

internal static class CourseExtensions
{
    public static CoursePublished ToCoursePublishedEvent(this Course course, IHashids hashids)
    {
        PublishedCourse publishedCourse = new()
        {
            OrganizationId = course.OrganizationId,
            CourseId = hashids.Encode(course.Id),
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            Duration = course.Modules.Select(x => x.ToPublishedCourseModule(hashids)).ToList().Sum(x => x.Duration),
            PublicationDate = course.PublicationDate!.Value,
            Modules = course.Modules.Select(x => x.ToPublishedCourseModule(hashids)).ToList(),
            CategoriesId = course.Categories.Select(x => x.CategoryId).ToList()
        };

        return new CoursePublished(publishedCourse);
    }

    private static PublishedCourseLecture ToPublishedCourseLecture(this Lecture lecture, IHashids hashids)
    {
        return new PublishedCourseLecture
        {
            Id = hashids.Encode(lecture.Id),
            Title = lecture.Title,
            LectureType = lecture.Type,
            Duration = lecture.Duration,
            Order = lecture.Order,
            ResourceId = lecture.ResourceId
        };
    }
    
    private static PublishedCourseModule ToPublishedCourseModule(this Module module, IHashids hashids)
    {
        List<PublishedCourseLecture> publishedLectures =
            module.Lectures.Select(x => x.ToPublishedCourseLecture(hashids)).ToList();
        int moduleDuration = publishedLectures.Sum(x => x.Duration);

        return new PublishedCourseModule
        {
            Id = hashids.Encode(module.Id),
            Title = module.Title,
            Duration = moduleDuration,
            Order = module.Order,
            Lectures = publishedLectures
        };
    }
}