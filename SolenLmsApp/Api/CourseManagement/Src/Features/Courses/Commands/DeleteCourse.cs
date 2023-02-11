using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Imanys.SolenLms.Application.Shared.Core.Events;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.DeleteCourse;

#region Web API

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Delete a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the course to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RequestResponse>> DeleteCourse(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new DeleteCourseCommand(courseId), cancellationToken));
    }
}

#endregion

public sealed record DeleteCourseCommand(string CourseId) : IRequest<RequestResponse>;

internal sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public DeleteCourseCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<DeleteCourseCommandHandler> logger,
        IIntegrationEventsSender eventsSender)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    #endregion
    
    public async Task<RequestResponse> Handle(DeleteCourseCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid");

            Course? course = await GetCourseFromRepository(courseId);
            if (course is null)
                return Error("The course does not exist.");

            await DeleteCourseFromRepository(course);

            await SendCourseDeletedEvent(course.OrganizationId, command.CourseId);

            return Ok("The course has been deleted.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while deleting the course.", ex);
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

    private async Task<Course?> GetCourseFromRepository(int courseId)
    {
        Course? course = await _repository.GetByIdAsync(courseId);
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task DeleteCourseFromRepository(Course course)
    {
        await _repository.DeleteAsync(course);

        _logger.LogInformation("Course deleted. courseId:{courseId}, encodedCourseId:{encodedCourseId}", course.Id,
            _hashids.Encode(course.Id));
    }

    private async Task SendCourseDeletedEvent(string organizationId, string encodedCourseId)
    {
        CourseDeleted courseDeletedEvent = new() { OrganizationId = organizationId, CourseId = encodedCourseId };

        await _eventsSender.SendEvent(courseDeletedEvent);
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while deleting the course. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}