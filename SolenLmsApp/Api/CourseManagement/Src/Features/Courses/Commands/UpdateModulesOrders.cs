using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.UpdateModulesOrders;

#region Web API

[Route("course-management/courses/{courseId}/modules")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Update the order of a training course modules
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="command">Object containing information about the new order od the modules</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("order")]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateModuleOrders(string courseId,
        UpdateModulesOrdersCommand? command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class UpdateModulesOrdersCommandValidator : AbstractValidator<UpdateModulesOrdersCommand>
{
    public UpdateModulesOrdersCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ModulesOrders).NotNull();
    }
}

#endregion

public sealed record UpdateModulesOrdersCommand : IRequest<RequestResponse>
{
    [JsonIgnore] public string CourseId { get; set; } = default!;
    public IEnumerable<ModuleOrder> ModulesOrders { get; set; } = default!;
}

public sealed record ModuleOrder(string ModuleId, int Order);

internal sealed class UpdateModulesOrdersCommandHandler : IRequestHandler<UpdateModulesOrdersCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateModulesOrdersCommandHandler> _logger;

    public UpdateModulesOrdersCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<UpdateModulesOrdersCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    #endregion
    
    public async Task<RequestResponse> Handle(UpdateModulesOrdersCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            Course? course = await LoadCourseWithModulesFromRepository(courseId);
            if (course is null)
                return Error("The course does not exist.");

            UpdateModulesOrders(course, command);

            await SaveCourseToRepository(course);

            return Ok("The modules order has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the modules orders.", ex);
        }
    }

    #region private methodes

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId)
    {
        if (_hashids.TryDecodeSingle(encodedCourseId, out courseId))
            return true;

        _logger.LogWarning("The encoded course id is invalid. encodedCourseId:{encodedCourseId}", encodedCourseId);
        return false;
    }

    private async Task<Course?> LoadCourseWithModulesFromRepository(int courseId)
    {
        Course? course =
            await _repository.FirstOrDefaultAsync(new GetCourseWithModulesSpec(courseId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private void UpdateModulesOrders(Course course, UpdateModulesOrdersCommand command)
    {
        foreach (Module module in course.Modules)
        {
            string? moduleId = _hashids.Encode(module.Id);

            int order = command.ModulesOrders.Any(m => m.ModuleId == moduleId)
                ? command.ModulesOrders.First(m => m.ModuleId == moduleId).Order
                : module.Order;

            module.UpdateOrder(order);
        }
    }

    private async Task SaveCourseToRepository(Course course)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Modules orders updated.");
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while reordering the modules of the course. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
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