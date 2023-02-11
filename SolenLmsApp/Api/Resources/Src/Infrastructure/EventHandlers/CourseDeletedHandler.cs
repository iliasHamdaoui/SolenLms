using Imanys.SolenLms.Application.Resources.Features;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.EventHandlers;

internal sealed class CourseDeletedHandler : INotificationHandler<CourseDeleted>
{
    private readonly ResourcesDbContext _dbContext;
    private readonly IMediaManager _mediaManager;
    private readonly ILogger<CourseDeletedHandler> _logger;

    public CourseDeletedHandler(ResourcesDbContext dbContext, IMediaManager mediaManager,
        ILogger<CourseDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _mediaManager = mediaManager;
        _logger = logger;
    }

    public async Task Handle(CourseDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteCourseResourcesFromRepository(@event.CourseId, cancellationToken);

            await _mediaManager.DeleteCourseMedias(@event.OrganizationId, @event.CourseId);

            _logger.LogInformation("Course resources deleted. courseId:{courseId}, count:{count}", @event.CourseId,
                count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting course resources. courseId:{courseId}, message:{message}",
                @event.CourseId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteCourseResourcesFromRepository(string courseId, CancellationToken cancellationToken)
    {
        return await _dbContext.Resources
            .Where(x => x.CourseId == courseId)
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}