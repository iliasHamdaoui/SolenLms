using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.UpdateCourse;

#region Web API

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// update information about a training course
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to update</param>
    /// <param name="command">Object containing information about the training course to update.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateCourse(string courseId, UpdateCourseCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.CourseTitle).NotEmpty().MaximumLength(60);
        RuleFor(x => x.CourseDescription).MaximumLength(200);
    }
}

#endregion

public sealed record UpdateCourseCommand : IRequest<RequestResponse>
{
    public string CourseTitle { get; set; } = default!;
    public string? CourseDescription { get; set; }
    [JsonIgnore] public string CourseId { get; set; } = default!;
}

internal sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(IRepository<Course> courseRepository, IHashids hashids,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
        _logger = logger;
    }

    #endregion

    public async Task<RequestResponse> Handle(UpdateCourseCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            Course? courseToUpdate = await GetCourseFromRepository(courseId);
            if (courseToUpdate is null)
                return Error("The course does not exist.");

            courseToUpdate.UpdateTitle(command.CourseTitle);
            courseToUpdate.UpdateDescription(command.CourseDescription);

            await SaveCourseToRepository(courseToUpdate);

            return Ok("The training course has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the course.", ex);
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
        Course? course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task SaveCourseToRepository(Course courseToUnpublish)
    {
        await _courseRepository.UpdateAsync(courseToUnpublish);

        _logger.LogInformation("Course updated. courseId:{courseId}, encodedCourseId:{encodedCourseId}",
            courseToUnpublish.Id, _hashids.Encode(courseToUnpublish.Id));
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while updating the course. message:{message}",
            exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}