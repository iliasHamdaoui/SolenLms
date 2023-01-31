using Imanys.SolenLms.Application.Learning.Core.Domain.Courses;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnersProgress;

namespace Imanys.SolenLms.Application.Learning.Features.Courses.Queries.GetCourseToLearnById;

using static RequestResponse<GetCourseToLearnByIdQueryResult>;
using Response = RequestResponse<GetCourseToLearnByIdQueryResult>;

#region Web API

[Route("learning")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to learn</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse<GetCourseToLearnByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetCourseToLearnByIdQueryResult>>> GetCourse(string courseId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseToLearnByIdQuery(courseId), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetCourseToLearnByIdQueryResult
{
    public string CourseId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public string? FirstLecture { get; set; }
    public float LearnerProgress { get; set; }
    public IEnumerable<ModuleForGetCourseToLearnByIdQueryResult> Modules { get; set; } = default!;
}

public sealed record ModuleForGetCourseToLearnByIdQueryResult
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public int Order { get; set; }
    public IEnumerable<LectureForGetCourseToLearnByIdQueryResult> Lectures { get; set; } = default!;
}

public sealed record LectureForGetCourseToLearnByIdQueryResult
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string LectureType { get; set; } = default!;
    public int Duration { get; set; }
    public int Order { get; set; }
    public string? ResourceId { get; set; }
    public string? PreviousLectureId { get; set; }
    public string? NextLectureId { get; set; }
    public string? Content { get; set; }
}

#endregion

public sealed record GetCourseToLearnByIdQuery(string CourseId) : IRequest<Response>;

internal sealed class GetCourseToLearnByIdQueryHandler : IRequestHandler<GetCourseToLearnByIdQuery, Response>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<LearnerCourseAccess> _lectureAccess;
    private readonly ICurrentUser _currentUser;

    public GetCourseToLearnByIdQueryHandler(IRepository<Course> courseRepository,
        IRepository<LearnerCourseAccess> lectureAccessRepo, ICurrentUser currentUser)
    {
        _courseRepository = courseRepository;
        _lectureAccess = lectureAccessRepo;
        _currentUser = currentUser;
    }

    #endregion

    public async Task<Response> Handle(GetCourseToLearnByIdQuery query, CancellationToken cancellationToken)
    {
        Course? course = await GetCourseFromRepository(query.CourseId, cancellationToken);

        if (course is null)
            return NotFound("The course does not exist.");

        LearnerCourseAccess? lastAccessedLecture = await GetLastAccessedCourseFromRepository(course, cancellationToken);

        return Ok(data: course.ToQueryResult(lastAccessedLecture));
    }

    #region private methods

    private async Task<Course?> GetCourseFromRepository(string courseId, CancellationToken cancellationToken)
    {
        return await _courseRepository.FirstOrDefaultAsync(new GetCourseByIdSpec(courseId, _currentUser.UserId),
            cancellationToken);
    }

    private async Task<LearnerCourseAccess?> GetLastAccessedCourseFromRepository(Course course,
        CancellationToken cancellationToken)
    {
        return await _lectureAccess.FirstOrDefaultAsync(new GetLearnerLectureAccessSpec(_currentUser.UserId, course.Id),
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
                .Include(course => course.LearnersProgress.Where(x => x.LearnerId == learnerId))
                .AsSplitQuery()
                .AsNoTracking();
        }
    }

    private sealed class GetLearnerLectureAccessSpec : SingleResultSpecification<LearnerCourseAccess>
    {
        public GetLearnerLectureAccessSpec(string learnerId, string courseId)
        {
            Query
                .Where(x => x.LearnerId == learnerId && x.CourseId == courseId)
                .OrderByDescending(x => x.AccessTime)
                .Take(1)
                .AsNoTracking();
        }
    }

    #endregion
}

#region extensions

internal static class CourseExtension
{
    public static GetCourseToLearnByIdQueryResult ToQueryResult(this Course course,
        LearnerCourseAccess? lastAccessedLecture)
    {
        IEnumerable<ModuleForGetCourseToLearnByIdQueryResult> modules = GetCourseModules(course);

        return new GetCourseToLearnByIdQueryResult
        {
            CourseId = course.Id,
            Title = course.Title,
            Duration = course.Duration,
            FirstLecture =
                lastAccessedLecture?.LectureId ?? modules.FirstOrDefault()?.Lectures.FirstOrDefault()?.Id,
            LearnerProgress = course.LearnersProgress.FirstOrDefault()?.Progress ?? 0,
            Modules = modules
        };
    }

    private static List<ModuleForGetCourseToLearnByIdQueryResult> GetCourseModules(Course course)
    {
        List<ModuleForGetCourseToLearnByIdQueryResult> modules = new();
        LectureForGetCourseToLearnByIdQueryResult? previousLecture = null;
        foreach (Module module in course.Modules.OrderBy(x => x.Order))
        {
            List<LectureForGetCourseToLearnByIdQueryResult> lectures = new();
            foreach (Lecture lecture in module.Lectures.OrderBy(x => x.Order))
            {
                LectureForGetCourseToLearnByIdQueryResult lectureToReturn = new()
                {
                    Id = lecture.Id,
                    Title = lecture.Title,
                    LectureType = lecture.Type.Value,
                    Order = lecture.Order,
                    Duration = lecture.Duration,
                    ResourceId = lecture.ResourceId,
                };

                if (previousLecture is not null)
                {
                    previousLecture.NextLectureId = lectureToReturn.Id;
                    lectureToReturn.PreviousLectureId = previousLecture.Id;
                }

                lectures.Add(lectureToReturn);
                previousLecture = lectureToReturn;
            }

            modules.Add(new ModuleForGetCourseToLearnByIdQueryResult
            {
                Id = module.Id,
                Title = module.Title,
                Duration = module.Duration,
                Order = module.Order,
                Lectures = lectures
            });
        }

        return modules;
    }
}

#endregion