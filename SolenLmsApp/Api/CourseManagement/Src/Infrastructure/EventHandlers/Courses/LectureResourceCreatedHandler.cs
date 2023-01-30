using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.Resources;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Courses;

internal sealed class LectureResourceCreatedHandler : INotificationHandler<LectureResourceCreated>
{
    private readonly CourseManagementDbContext _dbContext;
    private readonly IHashids _hashids;
    private readonly ILogger<LectureResourceCreatedHandler> _logger;

    public LectureResourceCreatedHandler(CourseManagementDbContext dbContext, IHashids hashids,
        ILogger<LectureResourceCreatedHandler> logger)
    {
        _dbContext = dbContext;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task Handle(LectureResourceCreated @event, CancellationToken cancellationToken)
    {
        if (!TryDecodeLectureId(@event.LectureId, out int lectureId))
        {
            _logger.LogWarning("The lecture id is invalid. encodedLectureId:{encodedLectureId}", @event.LectureId);
            return;
        }

        Lecture? lecture = await GetLectureToUpdateFromRepository(lectureId, cancellationToken);
        if (lecture is null)
        {
            _logger.LogWarning("he lecture does not exist. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}",
                lectureId, @event.LectureId);
            return;
        }

        try
        {
            lecture.SetResourceId(@event.ResourceId);

            await SaveChangesToRepository(cancellationToken);

            _logger.LogInformation(
                "The lecture's resource id has been set. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}",
                lectureId, @event.LectureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while setting the resource id of the lecture. encodedLectureId:{encodedLectureId}, message:{message}",
                @event.LectureId, ex.Message);
        }
    }

    #region private methods

    private bool TryDecodeLectureId(string encodedLectureId, out int lectureId) =>
        _hashids.TryDecodeSingle(encodedLectureId, out lectureId);

    private async Task<Lecture?> GetLectureToUpdateFromRepository(int lectureId, CancellationToken cancellationToken)
    {
        return await _dbContext.Lecture
            .FirstOrDefaultAsync(x => x.Id == lectureId, cancellationToken);
    }

    private async Task SaveChangesToRepository(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}