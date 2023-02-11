using Imanys.SolenLms.Application.Learning.Core.Domain.Courses;
using Imanys.SolenLms.Application.Learning.Core.Domain.CoursesCategories;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Courses;

internal sealed class CoursePublishedHandler : INotificationHandler<CoursePublished>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<CoursePublishedHandler> _logger;

    public CoursePublishedHandler(LearningDbContext dbContext, ILogger<CoursePublishedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CoursePublished @event, CancellationToken cancellationToken)
    {
        try
        {
            PublishedCourse publishedCourse = @event.PublishedCourse;

            await CreateOrUpdateCourseFromPublishedCourse(publishedCourse);

            await SaveChangesToRepository(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while handling course published event. courseId:{courseId}, message:{message}",
                @event.PublishedCourse.CourseId, ex.Message);
        }
    }


    #region private methods

    private async Task CreateOrUpdateCourseFromPublishedCourse(PublishedCourse publishedCourse)
    {
        Course? course = await GetCourseFromRepository(publishedCourse.CourseId);

        if (course is null)
        {
            course = new Course(publishedCourse.CourseId, publishedCourse.OrganizationId);
            AddCourseToRepository(course);
        }

        foreach (PublishedCourseModule publishedModule in publishedCourse.Modules)
        {
            Module module = course.AddOrUpdateModule(publishedModule.Id, publishedModule.Title, publishedModule.Order,
                publishedModule.Duration);
            foreach (PublishedCourseLecture lecture in publishedModule.Lectures)
                module.AddOrUpdateLecture(lecture.Id, lecture.Title, lecture.LectureType, lecture.Order,
                    lecture.Duration, lecture.ResourceId);
        }

        course.SetTitle(publishedCourse.Title);
        course.SetPublicationDate(publishedCourse.PublicationDate);
        course.SetDescription(publishedCourse.Description);
        course.SetDuration(publishedCourse.Duration);
        course.UpdateInstructor(publishedCourse.InstructorId);

        List<int> categoriesToAdd = publishedCourse.CategoriesId
            .Where(categoryId => course.Categories.All(x => x.CategoryId != categoryId))
            .ToList();

        foreach (int categoryId in categoriesToAdd)
            course.AddCategory(categoryId);

        List<CourseCategory> categoriesToRemove = course.Categories
            .Where(x => publishedCourse.CategoriesId.All(c => c != x.CategoryId))
            .ToList();

        foreach (CourseCategory category in categoriesToRemove)
            course.RemoveCategory(category);
    }

    private async Task<Course?> GetCourseFromRepository(string courseId)
    {
        return await _dbContext.Courses
            .Include(x => x.Modules)
            .ThenInclude(x => x.Lectures)
            .Include(x => x.Categories)
            .AsSplitQuery()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == courseId);
    }

    private void AddCourseToRepository(Course course)
    {
        _dbContext.Courses.Add(course);
    }

    private async Task SaveChangesToRepository(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}