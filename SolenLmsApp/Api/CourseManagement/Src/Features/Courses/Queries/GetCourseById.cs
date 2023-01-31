using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetCourseById;

using static RequestResponse<GetCourseByIdQueryResult>;

#region Web API

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to get</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse<GetCourseByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetCourseByIdQueryResult>>> GetCourseById(string courseId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseByIdQuery(courseId), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetCourseByIdQueryResult
{
    public string CourseId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public IEnumerable<ModuleForGetCourseByIdQueryResult> Modules { get; set; } = default!;
}

public sealed record LectureForGetCourseByIdQueryResult(string Id, string Title, string LectureType, int Duration,
    int Order, string? ResourceId);

public sealed record ModuleForGetCourseByIdQueryResult
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required int Duration { get; set; }
    public required int Order { get; set; }
    public required IEnumerable<LectureForGetCourseByIdQueryResult> Lectures { get; set; }
}

#endregion

public sealed record GetCourseByIdQuery(string CourseId) : IRequest<RequestResponse<GetCourseByIdQueryResult>>;

internal sealed class QueryHandler : IRequestHandler<GetCourseByIdQuery, RequestResponse<GetCourseByIdQueryResult>>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;

    public QueryHandler(IRepository<Course> courseRepository, IHashids hashids)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
    }

    #endregion
    
    public async Task<RequestResponse<GetCourseByIdQueryResult>> Handle(GetCourseByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryDecodeCourseId(query.CourseId, out int courseId))
            return NotFound("The course id is invalid.");

        Course? course = await GetCourseFromRepository(courseId, cancellationToken);

        if (course is null)
            return NotFound("The course does not exist.");

        return Ok(data: course.ToQueryResult(_hashids));
    }


    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId)
    {
        return _hashids.TryDecodeSingle(encodedCourseId, out courseId);
    }

    private async Task<Course?> GetCourseFromRepository(int courseId, CancellationToken cancellationToken)
    {
        return await _courseRepository.FirstOrDefaultAsync(new GetCourseByIdSpec(courseId), cancellationToken);
    }

    private sealed class GetCourseByIdSpec : SingleResultSpecification<Course>
    {
        public GetCourseByIdSpec(int courseId)
        {
            Query
                .Where(course => course.Id == courseId)
                .Include(course => course.Modules)
                .ThenInclude(module => module.Lectures)
                .Include(course => course.Instructor)
                .AsSplitQuery()
                .AsNoTracking();
        }
    }

    #endregion
}

#region extensions

internal static class CourseExtensions
{
    private static LectureForGetCourseByIdQueryResult ToLectureResult(this Lecture lecture, IHashids hashids)
    {
        return new LectureForGetCourseByIdQueryResult(hashids.Encode(lecture.Id), lecture.Title, lecture.Type.Value,
            lecture.Duration, lecture.Order, lecture.ResourceId);
    }

    private static ModuleForGetCourseByIdQueryResult ToModuleResult(this Module module, IHashids hashids)
    {
        List<LectureForGetCourseByIdQueryResult> lectures =
            module.Lectures.OrderBy(x => x.Order).Select(x => x.ToLectureResult(hashids)).ToList();

        return new ModuleForGetCourseByIdQueryResult
        {
            Id = hashids.Encode(module.Id),
            Title = module.Title,
            Duration = lectures.Sum(x => x.Duration),
            Order = module.Order,
            Lectures = lectures
        };
    }

    public static GetCourseByIdQueryResult ToQueryResult(this Course course, IHashids hashids)
    {
        List<ModuleForGetCourseByIdQueryResult> modules =
            course.Modules.OrderBy(x => x.Order).Select(x => x.ToModuleResult(hashids)).ToList();

        return new GetCourseByIdQueryResult
        {
            CourseId = hashids.Encode(course.Id),
            Title = course.Title,
            Description = course.Description,
            Duration = modules.Sum(x => x.Duration),
            IsPublished = course.IsPublished,
            PublicationDate = course.PublicationDate,
            InstructorId = course.InstructorId,
            InstructorName = course.Instructor?.FullName,
            CreatedAt = course.CreatedAt,
            LastModifiedAt = course.LastModifiedAt,
            Modules = modules
        };
    }
}

#endregion
