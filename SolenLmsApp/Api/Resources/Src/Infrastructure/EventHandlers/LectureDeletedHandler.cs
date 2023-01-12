using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.EventHandlers;

internal sealed class LectureDeletedHandler : INotificationHandler<LectureDeleted>
{
    private readonly ResourcesDbContext _dbContext;
    private readonly IMediaManager _mediaManager;
    private readonly ILogger<LectureDeletedHandler> _logger;

    public LectureDeletedHandler(ResourcesDbContext dbContext, IMediaManager mediaManager,
        ILogger<LectureDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _mediaManager = mediaManager;
        _logger = logger;
    }

    public async Task Handle(LectureDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteLectureResourcesFromRepository(@event.LectureId, cancellationToken);

            await _mediaManager.DeleteLectureMedias(@event.OrganizationId, @event.CourseId, @event.ModuleId,
                @event.LectureId);

            _logger.LogInformation(
                "Lecture resources deleted. courseId:{courseId}, lectureId:{lectureId}, count:{count}", @event.CourseId,
                @event.LectureId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting lecture resources. courseId:{courseId}, lectureId:{lectureId}, message:{message}",
                @event.CourseId, @event.LectureId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteLectureResourcesFromRepository(string lectureId, CancellationToken cancellationToken)
    {
        return await _dbContext.Resources
            .IgnoreQueryFilters()
            .Where(x => x.LectureId == lectureId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}