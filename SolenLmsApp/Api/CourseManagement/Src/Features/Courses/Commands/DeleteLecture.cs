using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.DeleteLecture;

#region Web API

[Route("course-management/courses/{courseId}/modules/{moduleId}/lectures")]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Delete a course lecture by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the lecture to delete belongs</param>
    /// <param name="moduleId">The id of the module to which the lecture to delete belongs</param>
    /// <param name="lectureId">The id of the lecture to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{lectureId}")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> DeleteLecture(string courseId, string moduleId, string lectureId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new DeleteLectureCommand(courseId, moduleId, lectureId), cancellationToken));
    }
}

#endregion

public sealed record DeleteLectureCommand
    (string CourseId, string ModuleId, string LectureId) : IRequest<RequestResponse>;

internal sealed class DeleteLectureCommandHandler : IRequestHandler<DeleteLectureCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<DeleteLectureCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public DeleteLectureCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<DeleteLectureCommandHandler> logger,
        IIntegrationEventsSender eventsSender)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    #endregion
    
    public async Task<RequestResponse> Handle(DeleteLectureCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("The module id is invalid.");

            if (!TryDecodeLectureId(command.LectureId, out int lectureId))
                return Error("The lecture id is invalid.");

            Course? course = await LoadCourseWithModuleAndLectureFromRepository(courseId, moduleId);
            if (course is null)
                return Error("The course does not exist.");

            Module? module = GetModuleToDeleteLectureFrom(moduleId, course);
            if (module is null)
                return Error("The module does not exists.");

            Lecture? lectureToDelete = GetLectureToDelete(lectureId, module);
            if (lectureToDelete is null)
                return Error("The lecture does not exists.");

            module.RemoveLecture(lectureToDelete);

            await SaveCourseToRepository(course, lectureId, command.LectureId);

            await SendLectureDeletedEvent(command, course);

            return Ok("The lecture has been deleted.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while deleting the lecture.", ex);
        }
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId)
    {
        if (_hashids.TryDecodeSingle(encodedCourseId, out courseId))
            return true;

        _logger.LogWarning("The encoded course id is invalid. encodedCourseId:{encodedCourseId}", encodedCourseId);
        return false;
    }

    private bool TryDecodeModuleId(string encodedModuleId, out int moduleId)
    {
        if (_hashids.TryDecodeSingle(encodedModuleId, out moduleId))
            return true;

        _logger.LogWarning("The encoded module id is invalid. encodedModuleId:{encodedModuleId}", encodedModuleId);
        return false;
    }

    private bool TryDecodeLectureId(string encodedLectureId, out int lectureId)
    {
        if (_hashids.TryDecodeSingle(encodedLectureId, out lectureId))
            return true;

        _logger.LogWarning("The encoded lecture id is invalid. encodedLectureId:{encodedLectureId}", encodedLectureId);
        return false;
    }

    private async Task<Course?> LoadCourseWithModuleAndLectureFromRepository(int courseId, int moduleId)
    {
        Course? course =
            await _repository.FirstOrDefaultAsync(new GetCourseWithModuleAndLectureSpec(courseId, moduleId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Module? GetModuleToDeleteLectureFrom(int moduleId, Course course)
    {
        Module? module = course.Modules.SingleOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private Lecture? GetLectureToDelete(int lectureId, Module module)
    {
        Lecture? lecture = module.Lectures.SingleOrDefault(x => x.Id == lectureId);
        if (lecture is null)
            _logger.LogWarning("The lecture does not exist. lectureId:{lectureId}", lectureId);

        return lecture;
    }

    private async Task SaveCourseToRepository(Course course, int lectureId, string encodedLectureId)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Lecture deleted. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}", lectureId,
            encodedLectureId);
    }

    private async Task SendLectureDeletedEvent(DeleteLectureCommand command, Course course)
    {
        LectureDeleted lectureDeletedEvent = new()
        {
            OrganizationId = course.OrganizationId,
            CourseId = command.CourseId,
            ModuleId = command.ModuleId,
            LectureId = command.LectureId
        };

        await _eventsSender.SendEvent(lectureDeletedEvent);
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while deleting the lecture. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private sealed class GetCourseWithModuleAndLectureSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleAndLectureSpec(int courseId, int moduleId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures);
        }
    }

    #endregion
}