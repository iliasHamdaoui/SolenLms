using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Courses;

internal sealed class CourseUnpublishedHandler : INotificationHandler<CourseUnpublished>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<CourseUnpublishedHandler> _logger;


    public CourseUnpublishedHandler(LearningDbContext dbContext, ILogger<CourseUnpublishedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CourseUnpublished @event, CancellationToken cancellationToken)
    {
        Course? courseToUnpublish = await GetCourseFromRepository(@event.CourseId, cancellationToken);
        if (courseToUnpublish is null)
        {
            _logger.LogWarning("Course not found. courseId:{courseId}", @event.CourseId);
            return;
        }

        try
        {
            courseToUnpublish.Unpublished();

            await SaveChangesToRepository(cancellationToken);

            _logger.LogWarning("Course unpublished. courseId:{courseId}", @event.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while unpublishing the course. courseId:{courseId}, message:{message}",
                @event.CourseId, ex.Message);
        }
    }

    #region private methods

    private async Task<Course?> GetCourseFromRepository(string courseId, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
    }

    private async Task SaveChangesToRepository(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}