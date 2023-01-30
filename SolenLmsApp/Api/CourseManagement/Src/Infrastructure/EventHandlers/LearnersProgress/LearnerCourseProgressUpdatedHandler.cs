using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerProgressAggregate;
using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.Learning;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.LearnersProgress;

internal sealed class LearnerCourseProgressUpdatedHandler : INotificationHandler<LearnerCourseProgressUpdated>
{
    private readonly CourseManagementDbContext _dbContext;
    private readonly IHashids _hashids;
    private readonly ILogger<LearnerCourseProgressUpdatedHandler> _logger;

    public LearnerCourseProgressUpdatedHandler(CourseManagementDbContext dbContext, IHashids hashids,
        ILogger<LearnerCourseProgressUpdatedHandler> logger)
    {
        _dbContext = dbContext;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task Handle(LearnerCourseProgressUpdated @event, CancellationToken token)
    {
        try
        {
            if (!TryDecodeCourseId(@event.CourseId, out int courseId))
            {
                _logger.LogWarning("Invalid course id. encodedCourseId:{encodedCourseId}", @event.CourseId);
                return;
            }

            LearnerCourseProgress progress = await GetLearnerProgressFromRepository(@event.LearnerId, courseId, token);

            progress.UpdateProgress(@event.Progress, @event.LastAccessTime);

            await SaveChangesToRepository(token);

            _logger.LogInformation("Learner progress updated. UserId:{UserId}, encodedCourseId:{encodedCourseId}",
                @event.LearnerId, @event.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while updating learner progress. UserId:{UserId}, encodedCourseId:{encodedCourseId}, message:{message}",
                @event.LearnerId, @event.CourseId, ex.Message);
        }
    }
    
    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId) =>
        _hashids.TryDecodeSingle(encodedCourseId, out courseId);

    private async Task<LearnerCourseProgress> GetLearnerProgressFromRepository(string learnerId, int courseId,
        CancellationToken cancellationToken)
    {
        LearnerCourseProgress? progress = await _dbContext.LearnerProgress.FirstOrDefaultAsync(
            x => x.LearnerId == learnerId && x.CourseId == courseId,
            cancellationToken);

        if (progress is not null)
            return progress;

        progress = new LearnerCourseProgress(learnerId, courseId);
        _dbContext.LearnerProgress.Add(progress);

        return progress;
    }

    private async Task SaveChangesToRepository(CancellationToken token)
    {
        await _dbContext.SaveChangesAsync(token);
    }

    #endregion
}