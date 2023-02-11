using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Courses;

internal sealed class CourseDeletedHandler : INotificationHandler<CourseDeleted>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<CourseDeletedHandler> _logger;


    public CourseDeletedHandler(LearningDbContext dbContext, ILogger<CourseDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CourseDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteCourseFromRepository(@event.CourseId, cancellationToken);

            _logger.LogInformation("Course deleted. courseId:{courseId}, count:{count}", @event.CourseId,
                count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while deleting course. courseId:{courseId}, message:{message}",
                @event.CourseId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteCourseFromRepository(string courseId, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .IgnoreQueryFilters()
            .Where(x => x.Id == courseId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}