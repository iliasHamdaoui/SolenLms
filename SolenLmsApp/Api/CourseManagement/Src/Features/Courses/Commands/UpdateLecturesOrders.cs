using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.UpdateLecturesOrders;

#region Web API

[Route("course-management/courses/{courseId}/modules/{moduleId}/lectures")]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Update the order of a training course lectures
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="moduleId">The id of the module to which the lectures belong</param>
    /// <param name="command">Object containing information about the new order od the modules</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("order")]
    [Authorize(Policy = CourseManagementPolicy)]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateLectureOrders(string courseId, string moduleId,
        UpdateLecturesOrdersCommand? command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;
        command.ModuleId = moduleId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class UpdateLecturesOrdersCommandValidator : AbstractValidator<UpdateLecturesOrdersCommand>
{
    public UpdateLecturesOrdersCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ModuleId).NotEmpty();
        RuleFor(x => x.LecturesOrders).NotNull();
    }
}

#endregion

public sealed class UpdateLecturesOrdersCommand : IRequest<RequestResponse>
{
    [JsonIgnore] public string CourseId { get; set; } = default!;
    [JsonIgnore] public string ModuleId { get; set; } = default!;
    public IEnumerable<LectureOrder> LecturesOrders { get; set; } = default!;
}

public sealed record LectureOrder(string LectureId, int Order);

internal sealed class UpdateLecturesOrdersCommandHandler : IRequestHandler<UpdateLecturesOrdersCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateLecturesOrdersCommandHandler> _logger;

    public UpdateLecturesOrdersCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<UpdateLecturesOrdersCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    #endregion
    
    public async Task<RequestResponse> Handle(UpdateLecturesOrdersCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("The module id is invalid.");

            Course? course = await LoadCourseWithModuleAndLecturesFromRepository(courseId, moduleId);
            if (course is null)
                return Error("The course does not exists.");

            Module? module = GetLecturesModule(moduleId, course);
            if (module is null)
                return Error("The module does not exists.");

            UpdateLecturesOrders(module, command);

            await SaveCourseToRepository(course);

            return Ok("The lectures order has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the lectures orders.", ex);
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

    private async Task<Course?> LoadCourseWithModuleAndLecturesFromRepository(int courseId, int moduleId)
    {
        Course? course =
            await _repository.FirstOrDefaultAsync(new GetCourseWithModuleAndLecturesSpec(courseId, moduleId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Module? GetLecturesModule(int moduleId, Course course)
    {
        Module? module = course.Modules.FirstOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private void UpdateLecturesOrders(Module module, UpdateLecturesOrdersCommand command)
    {
        foreach (Lecture lecture in module.Lectures)
        {
            string? lectureId = _hashids.Encode(lecture.Id);

            int order = command.LecturesOrders.Any(x => x.LectureId == lectureId)
                ? command.LecturesOrders.First(x => x.LectureId == lectureId).Order
                : module.Order;

            lecture.UpdateOrder(order);
        }
    }

    private async Task SaveCourseToRepository(Course course)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Lectures orders updated.");
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while reordering the lectures of the module. message:{message}",
            exception.Message);
        return Error(ResponseError.Unexpected, error);
    }


    private sealed class GetCourseWithModuleAndLecturesSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleAndLecturesSpec(int courseId, int moduleId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures);
        }
    }

    #endregion
}