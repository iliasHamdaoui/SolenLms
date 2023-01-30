using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.CreateLecture;

#region Web API

[Route("course-management/courses/{courseId}/modules/{moduleId}/lectures")]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Create a new training course lecture
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="moduleId">The id of the module</param>
    /// <param name="command">Object containing information about the lecture to create</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> CreateLecture(string courseId, string moduleId,
        CreateLectureCommand? command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;
        command.ModuleId = moduleId;

        RequestResponse<string> response = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction("GetLectureById", new { courseId, moduleId, lectureId = response.Data }, response);
    }
}

#endregion

#region Validator

public sealed class CreateLectureCommandValidator : AbstractValidator<CreateLectureCommand>
{
    public CreateLectureCommandValidator()
    {
        RuleFor(x => x.LectureTitle).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LectureType).NotEmpty();
    }
}

#endregion

public sealed record CreateLectureCommand : IRequest<RequestResponse<string>>
{
    [JsonIgnore] public string CourseId { get; set; } = default!;
    [JsonIgnore] public string ModuleId { get; set; } = default!;
    public string LectureTitle { get; set; } = default!;
    public string LectureType { get; set; } = default!;
}

internal sealed class CreateLectureCommandHandler : IRequestHandler<CreateLectureCommand, RequestResponse<string>>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<CreateLectureCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public CreateLectureCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<CreateLectureCommandHandler> logger, IIntegrationEventsSender eventsSender)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    #endregion


    public async Task<RequestResponse<string>> Handle(CreateLectureCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("Invalid module id.");

            if (!TryParseLectureType(command.LectureType, out LectureType lectureType))
                return Error("The lecture type is invalid.");

            Course? course = await LoadCourseWithModuleFromRepository(courseId, moduleId);
            if (course is null)
                return Error("The course does not exist.");

            Module? module = GetModuleToAddLectureTo(moduleId, course);
            if (module is null)
                return Error("The module does not exist.");

            Lecture createdLecture = module.AddLecture(command.LectureTitle, lectureType);

            await SaveCourseToRepository(course, createdLecture);

            if (LectureTypeIsArticleOrVideo(createdLecture))
                await SendLectureWithResourceCreatedEvent(command, course, createdLecture);

            return Ok("The lecture has been created.", _hashids.Encode(createdLecture.Id));
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the lecture.", ex);
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

    private bool TryParseLectureType(string lectureTypeValue, out LectureType lectureType)
    {
        if (Enumeration.TryConvertFromValue(lectureTypeValue, out lectureType))
            return true;

        _logger.LogWarning("The lecture type value is invalid. lectureTypeValue:{lectureTypeValue}", lectureTypeValue);

        return false;
    }

    private Module? GetModuleToAddLectureTo(int moduleId, Course course)
    {
        Module? module = course.Modules.FirstOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private async Task<Course?> LoadCourseWithModuleFromRepository(int courseId, int moduleId)
    {
        Course? course = await _repository.FirstOrDefaultAsync(new GetCourseWithModuleSpec(courseId, moduleId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task SaveCourseToRepository(Course? course, Lecture createdLecture)
    {
        await _repository.UpdateAsync(course!);

        _logger.LogInformation("Lecture created. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}",
            createdLecture.Id, _hashids.Encode(createdLecture.Id));
    }

    private async Task SendLectureWithResourceCreatedEvent(CreateLectureCommand command, Course course,
        Lecture createdLecture)
    {
        LectureWithResourceCreated @event = new()
        {
            OrganizationId = course.OrganizationId,
            CourseId = command.CourseId,
            ModuleId = command.ModuleId,
            LectureId = _hashids.Encode(createdLecture.Id),
            MediaType = createdLecture.Type.MediaType
        };

        await _eventsSender.SendEvent(@event);
    }

    private static bool LectureTypeIsArticleOrVideo(Lecture createdLecture)
    {
        return createdLecture.Type.Value == LectureType.Article.Value ||
               createdLecture.Type.Value == LectureType.Video.Value;
    }

    private RequestResponse<string> UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while creating the lecture. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private sealed class GetCourseWithModuleSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleSpec(int courseId, int moduleId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures);
        }
    }

    #endregion
}