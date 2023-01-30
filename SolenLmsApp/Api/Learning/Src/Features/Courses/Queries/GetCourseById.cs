using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;

namespace Imanys.SolenLms.Application.Learning.Features.Courses.Queries.GetCourseById;

using static RequestResponse<GetCourseByIdQueryResult>;
using Response = RequestResponse<GetCourseByIdQueryResult>;

#region Web API

[Route("courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to get information about</param>
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
    public DateTime PublicationDate { get; set; }
    public string? InstructorName { get; set; }
    public bool IsBookmarked { get; set; }
    public float LearnerProgress { get; set; }
    public IEnumerable<string> Categories { get; set; } = default!;
    public IEnumerable<ModuleForGetCourseByIdQueryResult> Modules { get; set; } = default!;
}

public sealed record ModuleForGetCourseByIdQueryResult
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public int Order { get; set; }
    public IEnumerable<LectureForGetCourseByIdQueryResult> Lectures { get; set; } = default!;
}

public sealed record LectureForGetCourseByIdQueryResult
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string LectureType { get; set; } = default!;
    public int Duration { get; set; }
    public int Order { get; set; }
}

#endregion

public sealed record GetCourseByIdQuery(string CourseId) : IRequest<RequestResponse<GetCourseByIdQueryResult>>;

internal sealed class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Response>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly ICurrentUser _currentUser;

    public GetCourseByIdQueryHandler(IRepository<Course> courseRepository, ICurrentUser currentUser)
    {
        _courseRepository = courseRepository;
        _currentUser = currentUser;
    }

    #endregion

    public async Task<Response> Handle(GetCourseByIdQuery query, CancellationToken cancellationToken)
    {
        Course? course = await GetCourseFromRepository(query.CourseId, cancellationToken);
        if (course is null)
            return NotFound("The course does not exist.");

        return Ok(data: course.ToQueryResult());
    }

    #region private methods

    private async Task<Course?> GetCourseFromRepository(string courseId, CancellationToken cancellationToken)
    {
        return await _courseRepository.FirstOrDefaultAsync(new GetCourseByIdSpec(courseId, _currentUser.UserId),
            cancellationToken);
    }

    private sealed class GetCourseByIdSpec : SingleResultSpecification<Course>
    {
        public GetCourseByIdSpec(string courseId, string learnerId)
        {
            Query
                .Where(course => course.Id == courseId && course.IsPublished)
                .Include(course => course.Modules)
                .ThenInclude(module => module.Lectures)
                .Include(course => course.Instructor)
                .Include(course => course.Categories)
                .ThenInclude(category => category.Category)
                .Include(course => course.LearnersBookmarks.Where(x => x.LearnerId == learnerId))
                .Include(course => course.LearnersProgress.Where(x => x.LearnerId == learnerId))
                .AsSplitQuery()
                .AsNoTracking();
        }
    }

    #endregion
}

#region extensions

internal static class CourseExtension
{
    private static LectureForGetCourseByIdQueryResult ToLectureResult(this Lecture lecture)
    {
        return new LectureForGetCourseByIdQueryResult
        {
            Id = lecture.Id,
            Title = lecture.Title,
            LectureType = lecture.Type.Value,
            Order = lecture.Order,
            Duration = lecture.Duration
        };
    }

    private static ModuleForGetCourseByIdQueryResult ToModuleResult(this Module module)
    {
        return new ModuleForGetCourseByIdQueryResult
        {
            Id = module.Id,
            Title = module.Title,
            Duration = module.Duration,
            Order = module.Order,
            Lectures = module.Lectures.Select(x => x.ToLectureResult()).OrderBy(x => x.Order).ToList()
        };
    }

    public static GetCourseByIdQueryResult ToQueryResult(this Course course)
    {
        return new GetCourseByIdQueryResult
        {
            CourseId = course.Id,
            Title = course.Title,
            Description = course.Description,
            Duration = course.Duration,
            PublicationDate = course.PublicationDate,
            InstructorName = course.Instructor?.FullName,
            IsBookmarked = course.LearnersBookmarks.Any(),
            LearnerProgress = course.LearnersProgress.FirstOrDefault()?.Progress ?? 0,
            Categories = course.Categories.Select(x => x.Category.Name),
            Modules = course.Modules.Select(x => x.ToModuleResult()).OrderBy(x => x.Order).ToList()
        };
    }
}

#endregion