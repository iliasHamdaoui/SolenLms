using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.CreateModule;

#region Web API

[Route("course-management/courses/{courseId}/modules")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Create a new training course module
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="command">Object containing information about the module to create</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> CreateModule(string courseId, CreateModuleCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        var response = await Mediator.Send(command, cancellationToken);

        return CreatedAtAction("GetModuleById", new { courseId, moduleId = response.Data }, response);
    }
}

#endregion

#region Validator

public sealed class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(x => x.ModuleTitle).NotEmpty().MaximumLength(60);
    }
}

#endregion

public sealed record CreateModuleCommand : IRequest<RequestResponse<string>>
{
    public string ModuleTitle { get; set; } = default!;
    [JsonIgnore] public string CourseId { get; set; } = default!;
};

internal sealed class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, RequestResponse<string>>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<CreateModuleCommandHandler> _logger;

    public CreateModuleCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<CreateModuleCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    #endregion
    
    public async Task<RequestResponse<string>> Handle(CreateModuleCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            Course? course = await GetCourseFromRepository(courseId);
            if (course is null)
                return Error("The course does not exist.");

            Module createdModule = course.AddModule(command.ModuleTitle);

            await SaveCourseToRepository(course, createdModule);

            return Ok("The module has been created.", _hashids.Encode(createdModule.Id));
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the module.", ex);
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


    private async Task SaveCourseToRepository(Course course, Module createdModule)
    {
        await _repository.UpdateAsync(course);
        _logger.LogInformation("Module created. moduleId:{moduleId}, encodedModuleId:{encodedModuleId}",
            createdModule.Id,
            _hashids.Encode(createdModule.Id));
    }

    private RequestResponse<string> UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while creating the module. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private async Task<Course?> GetCourseFromRepository(int courseId)
    {
        Course? course = await _repository.FirstOrDefaultAsync(new GetCourseWithModulesSpec(courseId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private sealed class GetCourseWithModulesSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModulesSpec(int courseId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules);
        }
    }

    #endregion
}