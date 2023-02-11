using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Imanys.SolenLms.Application.Shared.Core.Events;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.DeleteModule;

#region Web API

[Route("course-management/courses/{courseId}/modules")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Delete a course module by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the module to delete belongs</param>
    /// <param name="moduleId">The id of the module to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{moduleId}")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> DeleteModule(string courseId, string moduleId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new DeleteModuleCommand(courseId, moduleId), cancellationToken));
    }
}

#endregion

public sealed record DeleteModuleCommand(string CourseId, string ModuleId) : IRequest<RequestResponse>;

internal sealed class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<DeleteModuleCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public DeleteModuleCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<DeleteModuleCommandHandler> logger,
        IIntegrationEventsSender eventsSender)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    #endregion
    
    public async Task<RequestResponse> Handle(DeleteModuleCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("The module id is invalid.");

            Course? course = await LoadCourseWithTheModuleFromRepository(courseId);
            if (course is null)
                return Error("The course does not exists.");

            Module? moduleToDelete = GetModuleToDelete(moduleId, course);
            if (moduleToDelete is null)
                return Error("The module does not exists.");

            course.RemoveModule(moduleToDelete);

            await SaveCourseToRepository(course, moduleId, command.ModuleId);

            await SendModuleDeletedEvent(command, course);

            return Ok("The module has been deleted.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while deleting the module.", ex);
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

    private async Task<Course?> LoadCourseWithTheModuleFromRepository(int courseId)
    {
        Course? course = await _repository.FirstOrDefaultAsync(new GetCourseWithModuleSpec(courseId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Module? GetModuleToDelete(int moduleId, Course course)
    {
        Module? module = course.Modules.SingleOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private async Task SaveCourseToRepository(Course course, int moduleId, string encodedModuleId)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Module deleted. moduleId:{moduleId}, encodedModuleId:{encodedModuleId}", moduleId,
            encodedModuleId);
    }

    private async Task SendModuleDeletedEvent(DeleteModuleCommand command, Course course)
    {
        ModuleDeleted moduleDeletedEvent = new()
        {
            OrganizationId = course.OrganizationId, CourseId = command.CourseId, ModuleId = command.ModuleId
        };

        await _eventsSender.SendEvent(moduleDeletedEvent);
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while deleting the module. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private sealed class GetCourseWithModuleSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleSpec(int courseId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules);
        }
    }

    #endregion
}