using HashidsNet;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Courses;

internal sealed class LectureResourceContentUpdatedHandler : INotificationHandler<LectureResourceContentUpdated>
{
    private readonly CourseManagementDbContext _dbContext;
    private readonly ILogger<LectureResourceContentUpdatedHandler> _logger;
    private readonly IHashids _hashids;

    public LectureResourceContentUpdatedHandler(CourseManagementDbContext dbContext,
        ILogger<LectureResourceContentUpdatedHandler> logger, IHashids hashids)
    {
        _dbContext = dbContext;
        _logger = logger;
        _hashids = hashids;
    }

    public async Task Handle(LectureResourceContentUpdated @event, CancellationToken cancellationToken)
    {
        Lecture? lecture = await GetLectureToUpdateByResourceIdFromRepository(@event.ResourceId, cancellationToken);
        if (lecture is null)
        {
            _logger.LogWarning("No lecture found. resourceId:{resourceId}", @event.ResourceId);
            return;
        }

        string? encodedLectureId = null;

        try
        {
            encodedLectureId = _hashids.Encode(lecture.Id);

            lecture.UpdateDuration(@event.Duration);

            await SaveChangesToRepository(cancellationToken);

            _logger.LogInformation(
                "Lecture duration updated. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}",
                lecture.Id, encodedLectureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while updating lecture duration. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}, message:{message}",
                lecture.Id, encodedLectureId, ex.Message);
        }
    }

    #region private methods

    private async Task<Lecture?> GetLectureToUpdateByResourceIdFromRepository(string resourceId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Lecture.FirstOrDefaultAsync(x => x.ResourceId == resourceId, cancellationToken);
    }

    private async Task SaveChangesToRepository(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}