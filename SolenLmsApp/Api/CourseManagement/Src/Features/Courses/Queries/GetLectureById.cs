using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetLectureById;

using static RequestResponse<GetLectureByIdQueryResult>;
using Response = RequestResponse<GetLectureByIdQueryResult>;

#region Web API

[Route("course-management/courses/{courseId}/modules/{moduleId}/lectures")]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a course lecture by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the lecture to get belongs</param>
    /// <param name="moduleId">The id of the module to which the module to get belongs</param>
    /// <param name="lectureId">The id of the lecture we want to get</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{lectureId}")]
    [ProducesResponseType(typeof(RequestResponse<GetLectureByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetLectureByIdQueryResult>>> GetLectureById(string courseId,
        string moduleId, string lectureId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetLectureByIdQuery(courseId, moduleId, lectureId), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetLectureByIdQueryResult
{
    public string Title { get; set; } = default!;
    public string? Type { get; set; }
    public string? ResourceId { get; set; }
    public string? MediaType { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
}

#endregion

public sealed record GetLectureByIdQuery(string CourseId, string ModuleId, string LectureId) : IRequest<Response>;

internal sealed class GetLectureByIdQueryHandler : IRequestHandler<GetLectureByIdQuery, Response>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;

    public GetLectureByIdQueryHandler(IRepository<Course> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    #endregion

    public async Task<Response> Handle(GetLectureByIdQuery query, CancellationToken token)
    {
        if (!TryDecodeCourseId(query.CourseId, out int courseId))
            return NotFound("The course id is invalid.");

        if (!TryDecodeModuleId(query.ModuleId, out int moduleId))
            return NotFound("The module id is invalid.");

        if (!TryDecodeLectureId(query.LectureId, out int lectureId))
            return NotFound("The lecture id is invalid.");

        Course? course = await LoadCourseWithModuleAndLectureFromRepository(courseId, moduleId, lectureId, token);
        if (course is null)
            return NotFound("The course does not exist.");

        Module? module = GetTheLectureModule(course, moduleId);
        if (module is null)
            return NotFound("The module does not exist.");

        Lecture? lecture = GetTheLecture(module, lectureId);
        if (lecture is null)
            return NotFound("The lecture does not exist.");

        return Ok(data: lecture.ToQueryResult());
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId) =>
        _hashids.TryDecodeSingle(encodedCourseId, out courseId);

    private bool TryDecodeModuleId(string encodedModuleId, out int moduleId) =>
        _hashids.TryDecodeSingle(encodedModuleId, out moduleId);


    private bool TryDecodeLectureId(string encodedLectureId, out int lectureId) =>
        _hashids.TryDecodeSingle(encodedLectureId, out lectureId);

    private static Module? GetTheLectureModule(Course course, int moduleId)
    {
        return course.Modules.FirstOrDefault(x => x.Id == moduleId);
    }

    private static Lecture? GetTheLecture(Module module, int lectureId)
    {
        return module.Lectures.FirstOrDefault(x => x.Id == lectureId);
    }

    private async Task<Course?> LoadCourseWithModuleAndLectureFromRepository(int courseId, int moduleId, int lectureId,
        CancellationToken cancellationToken)
    {
        return await _repository.FirstOrDefaultAsync(new GetLectureByIdSpec(courseId, moduleId, lectureId),
            cancellationToken);
    }

    private sealed class GetLectureByIdSpec : SingleResultSpecification<Course>
    {
        public GetLectureByIdSpec(int courseId, int moduleId, int lectureId)
        {
            Query.Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures.Where(lecture => lecture.Id == lectureId))
                .AsNoTracking();
        }
    }

    #endregion
}

#region extensions

internal static class LectureExtension
{
    public static GetLectureByIdQueryResult ToQueryResult(this Lecture lecture)
    {
        return new GetLectureByIdQueryResult
        {
            Title = lecture.Title,
            Duration = lecture.Duration,
            Order = lecture.Order,
            ResourceId = lecture.ResourceId,
            MediaType = lecture.Type?.MediaType?.Value,
            Type = lecture.Type?.Value
        };
    }
}

#endregion
