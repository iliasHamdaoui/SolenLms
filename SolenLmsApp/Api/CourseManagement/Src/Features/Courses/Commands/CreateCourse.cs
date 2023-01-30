using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.CreateCourse;

#region Web Api

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Create a new training course
    /// </summary>
    /// <param name="command">Object containing information about the training course to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse<string>>> CreateCourse(CreateCourseCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        RequestResponse<string> response = await Mediator.Send(command, cancellationToken);

        return CreatedAtAction("GetCourseById", new { courseId = response.Data }, response);
    }
}

#endregion

#region Validator

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.CourseTitle).NotEmpty().MaximumLength(60);
    }
}

#endregion

public sealed record CreateCourseCommand : IRequest<RequestResponse<string>>
{
    public string CourseTitle { get; set; } = default!;
    public string? CourseDescription { get; set; }
}

internal sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, RequestResponse<string>>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(IRepository<Course> courseRepository, IHashids hashids, ICurrentUser currentUser,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
        _currentUser = currentUser;
        _logger = logger;
    }

    #endregion

    public async Task<RequestResponse<string>> Handle(CreateCourseCommand command, CancellationToken _)
    {
        try
        {
            Course newCourse = CreateNewCourse(command.CourseTitle);

            newCourse.UpdateDescription(command.CourseDescription);

            SetCurrentUserAsInstructorByDefault(newCourse);

            await AddNewCourseToRepository(newCourse);

            return Ok("The course has been created", _hashids.Encode(newCourse.Id));
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the course.", ex);
        }
    }

    #region private methods

    private Course CreateNewCourse(string title)
    {
        return new Course(_currentUser.OrganizationId, title);
    }

    private async Task AddNewCourseToRepository(Course newCourse)
    {
        await _courseRepository.AddAsync(newCourse);

        _logger.LogInformation("Course created. courseId:{courseId}, encodedCourseId:{encodedCourseId}", newCourse.Id,
            _hashids.Encode(newCourse.Id));
    }

    private void SetCurrentUserAsInstructorByDefault(Course newCourse)
    {
        newCourse.UpdateInstructorId(_currentUser.UserId);
    }

    private RequestResponse<string> UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while creating the course. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}