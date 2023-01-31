using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetModuleById;

using static RequestResponse<GetModuleByIdQueryResult>;
using Response = RequestResponse<GetModuleByIdQueryResult>;

#region Web API

[Route("course-management/courses/{courseId}/modules")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a course module by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the module to get belongs</param>
    /// <param name="moduleId">The id of the module we want to get</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{moduleId}")]
    [ProducesResponseType(typeof(RequestResponse<GetModuleByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetModuleByIdQueryResult>>> GetModuleById(string courseId,
        string moduleId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetModuleByIdQuery(courseId, moduleId), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetModuleByIdQueryResult
{
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public int Order { get; set; }
    public IEnumerable<LectureForGetModuleByIdQueryResult> Lectures { get; set; } = default!;
}

public sealed record LectureForGetModuleByIdQueryResult
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? ResourceId { get; set; }
    public int Duration { get; set; }
}

#endregion

public sealed record GetModuleByIdQuery(string CourseId, string ModuleId) : IRequest<Response>;

internal sealed class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery, Response>
{
    #region Constructor

    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;

    public GetModuleByIdQueryHandler(IRepository<Course> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    #endregion

    public async Task<Response> Handle(GetModuleByIdQuery query, CancellationToken cancellationToken)
    {
        if (!TryDecodeCourseId(query.CourseId, out int courseId))
            return NotFound("The course id is invalid.");

        if (!TryDecodeModuleId(query.ModuleId, out int moduleId))
            return NotFound("The module id is invalid.");

        Course? course = await LoadCourseWithModuleFromRepository(courseId, moduleId, cancellationToken);
        if (course is null)
            return NotFound("The course does not exist.");

        Module? module = GetTheModule(course, moduleId);
        if (module is null)
            return NotFound("The module does not exist.");

        return Ok(data: module.ToQueryResult(_hashids));
    }


    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId) =>
        _hashids.TryDecodeSingle(encodedCourseId, out courseId);

    private bool TryDecodeModuleId(string encodedModuleId, out int moduleId) =>
        _hashids.TryDecodeSingle(encodedModuleId, out moduleId);

    private async Task<Course?> LoadCourseWithModuleFromRepository(int courseId, int moduleId,
        CancellationToken cancellationToken)
    {
        return await _repository.FirstOrDefaultAsync(new GetModuleByIdSpec(courseId, moduleId), cancellationToken);
    }

    private static Module? GetTheModule(Course course, int moduleId)
    {
        return course.Modules.FirstOrDefault(x => x.Id == moduleId);
    }

    private sealed class GetModuleByIdSpec : SingleResultSpecification<Course>
    {
        public GetModuleByIdSpec(int courseId, int moduleId)
        {
            Query.Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures)
                .AsNoTracking();
        }
    }

    #endregion
}

#region extensions

internal static class ModuleExtensions
{
    private static LectureForGetModuleByIdQueryResult ToLectureResult(this Lecture lecture, IHashids hashids)
    {
        return new LectureForGetModuleByIdQueryResult
        {
            Id = hashids.Encode(lecture.Id),
            Duration = lecture.Duration,
            Title = lecture.Title,
            ResourceId = lecture.ResourceId,
            Type = lecture.Type.Value
        };
    }

    public static GetModuleByIdQueryResult ToQueryResult(this Module module, IHashids hashids)
    {
        List<LectureForGetModuleByIdQueryResult> lectures =
            module.Lectures.OrderBy(x => x.Order).Select(x => x.ToLectureResult(hashids)).ToList();


        return new GetModuleByIdQueryResult
        {
            Title = module.Title,
            Order = module.Order,
            Duration = lectures.Sum(x => x.Duration),
            Lectures = lectures
        };
    }
}

#endregion