using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.Learning;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Microsoft.Extensions.Logging;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.LearnersProgress.Commands.UpdateLearnerProgress;

internal sealed class
    UpdateLearnerProgressCommandHandler : IRequestHandler<UpdateLearnerProgressCommand, RequestResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly ILogger<UpdateLearnerProgressCommandHandler> _logger;
    private readonly IUpdateLearnerProgressRepo _repo;
    private readonly IIntegratedEventsSender _eventsSender;

    public UpdateLearnerProgressCommandHandler(ICurrentUser currentUser, IDateTime dateTime,
        ILogger<UpdateLearnerProgressCommandHandler> logger,
        IUpdateLearnerProgressRepo repo, IIntegratedEventsSender eventsSender)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
        _logger = logger;
        _repo = repo;
        _eventsSender = eventsSender;
    }

    public async Task<RequestResponse> Handle(UpdateLearnerProgressCommand command, CancellationToken _)
    {
        try
        {
            Course? course = await GetCourseFromRepository(command.CourseId);
            if (course is null)
                return Error("The course does not exist.");

            LearnerCourseAccess courseAccess = await UpdateCourseAccessTime(command);

            float progress = await CalculateAndUpdateLearnerProgress(command.CourseId, course, courseAccess.AccessTime);

            await SendLearnerCourseProgressUpdatedEvent(command.CourseId, progress, courseAccess.AccessTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A error occured while updating the learner lecture access. message:{message}",
                ex.Message);
        }

        return Ok();
    }

    #region private methods

    private async Task<Course?> GetCourseFromRepository(string courseId)
    {
        Course? course = await _repo.GetCourse(courseId);
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task<float> CalculateAndUpdateLearnerProgress(string courseId, Course course,
        DateTime lastAccessTime)
    {
        int completedDuration = await _repo.GetCourseCompletedDuration(_currentUser.UserId, courseId);

        float progress = 0f;
        if (completedDuration > 0)
            progress = completedDuration / (float)course.Duration;

        LearnerCourseProgress? learnerCourseProgress =
            await _repo.GetLearnerCourseProgress(_currentUser.UserId, courseId);

        if (learnerCourseProgress is null)
        {
            learnerCourseProgress = new LearnerCourseProgress(_currentUser.UserId, courseId);
            _repo.AddLearnerCourseProgress(learnerCourseProgress);
        }

        learnerCourseProgress.UpdateProgress(progress, lastAccessTime);

        await _repo.SaveChanges();

        _logger.LogInformation("The learner lecture access has been updated. courseId:{courseId}", courseId);

        return progress;
    }

    private async Task<LearnerCourseAccess> UpdateCourseAccessTime(UpdateLearnerProgressCommand command)
    {
        LearnerCourseAccess? learnerCourseAccess =
            await _repo.GetLearnerCourseAccess(_currentUser.UserId, command.CourseId, command.LectureId);
        if (learnerCourseAccess is null)
        {
            learnerCourseAccess = new LearnerCourseAccess(_currentUser.UserId, command.LectureId, command.CourseId);
            _repo.AddLearnerCourseAccess(learnerCourseAccess);
        }

        learnerCourseAccess.UpdateAccessTime(_dateTime.Now);

        if (command.LastLecture)
            learnerCourseAccess.SetCompleted();

        await _repo.SaveChanges();

        await _repo.MarkOtherLecturesAsComplete(_currentUser.UserId, command.CourseId, command.LectureId);

        return learnerCourseAccess;
    }

    private async Task SendLearnerCourseProgressUpdatedEvent(string courseId, float progress, DateTime lastAccessTime)
    {
        LearnerCourseProgressUpdated eventToSend = new()
        {
            LearnerId = _currentUser.UserId, CourseId = courseId, Progress = progress, LastAccessTime = lastAccessTime,
        };

        await _eventsSender.SendEvent(eventToSend);
    }

    #endregion
}