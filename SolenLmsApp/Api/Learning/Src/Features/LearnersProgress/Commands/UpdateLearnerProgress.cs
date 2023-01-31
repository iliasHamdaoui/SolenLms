using Imanys.SolenLms.Application.Learning.Core.Domain.Courses;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnersProgress;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.Learning;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Features.LearnersProgress.Commands.UpdateLearnerProgress;

#region Web API

[Route("learning")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Update learner progress
    /// </summary>
    /// <param name="courseId">The id of the course</param>
    /// <param name="lectureId">The id of the last lecture the learner had access to</param>
    /// <param name="lastLecture">Indicate if this the last lecture of the course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpPut("{courseId}/{lectureId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> UpdateLearnerProgress(string courseId, string lectureId, bool lastLecture, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new UpdateLearnerProgressCommand(courseId, lectureId, lastLecture), cancellationToken));
    }
}

#endregion

public sealed record UpdateLearnerProgressCommand
    (string CourseId, string LectureId, bool LastLecture) : IRequest<RequestResponse>;

internal sealed class CommandHandler : IRequestHandler<UpdateLearnerProgressCommand, RequestResponse>
{
    #region Constructor

    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IUpdateLearnerProgressRepo _repo;
    private readonly IIntegrationEventsSender _eventsSender;

    public CommandHandler(ICurrentUser currentUser, IDateTime dateTime,
        ILogger<CommandHandler> logger,
        IUpdateLearnerProgressRepo repo, IIntegrationEventsSender eventsSender)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
        _logger = logger;
        _repo = repo;
        _eventsSender = eventsSender;
    }

    #endregion
    
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

#region Repository

internal interface IUpdateLearnerProgressRepo
{
    void AddLearnerCourseAccess(LearnerCourseAccess learnerCourseAccess);
    Task<LearnerCourseAccess?> GetLearnerCourseAccess(string learnerId, string courseId, string lectureId);
    Task MarkOtherLecturesAsComplete(string learnerId, string courseId, string lectureId);
    Task<LearnerCourseProgress?> GetLearnerCourseProgress(string learnerId, string courseId);
    void AddLearnerCourseProgress(LearnerCourseProgress learnerCourseProgress);
    Task<int> GetCourseCompletedDuration(string learnerId, string courseId);
    Task<Course?> GetCourse(string courseId);

    Task SaveChanges();
}

internal sealed class UpdateLearnerProgressRepo : IUpdateLearnerProgressRepo
{
    private readonly LearningDbContext _dbContext;

    public UpdateLearnerProgressRepo(LearningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddLearnerCourseAccess(LearnerCourseAccess learnerCourseAccess)
    {
        _dbContext.LearnerCourseAccess.Add(learnerCourseAccess);
    }

    public void AddLearnerCourseProgress(LearnerCourseProgress learnerCourseProgress)
    {
        _dbContext.LearnerCourseProgress.Add(learnerCourseProgress);
    }

    public Task<Course?> GetCourse(string courseId)
    {
        return _dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == courseId);
    }

    public Task<LearnerCourseAccess?> GetLearnerCourseAccess(string learnerId, string courseId, string lectureId)
    {
        return _dbContext.LearnerCourseAccess.FirstOrDefaultAsync(x => x.LearnerId == learnerId && x.CourseId == courseId && x.LectureId == lectureId);
    }

    public Task<LearnerCourseProgress?> GetLearnerCourseProgress(string learnerId, string courseId)
    {
        return _dbContext.LearnerCourseProgress.FirstOrDefaultAsync(x => x.LearnerId == learnerId && x.CourseId == courseId);
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Task MarkOtherLecturesAsComplete(string learnerId, string courseId, string lectureId)
    {
        return _dbContext.LearnerCourseAccess
            .Where(x => x.LearnerId == learnerId && x.CourseId == courseId && x.LectureId != lectureId)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsCompleted, true));
    }

    public Task<int> GetCourseCompletedDuration(string learnerId, string courseId)
    {
        return _dbContext.LearnerCourseAccess.Where(x => x.LearnerId == learnerId && x.CourseId == courseId && x.IsCompleted).Include(x => x.Lecture).SumAsync(x => x.Lecture.Duration);
    }
}

#endregion