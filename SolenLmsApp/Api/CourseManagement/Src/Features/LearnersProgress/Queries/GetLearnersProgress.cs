using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerAggregate;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerProgressAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Features.LearnersProgress.Queries.GetLearnersProgress;

using static RequestResponse<GetLearnersProgressQueryResult>;
using Response = RequestResponse<GetLearnersProgressQueryResult>;

#region Web API

[Route("course-management/courses/{courseId}/learners")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get course learners progress
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = CourseManagementPolicy)]
    [ProducesResponseType(typeof(RequestResponse<GetLearnersProgressQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetLearnersProgressQueryResult>>> GetLearnersProgress(
        string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetLearnersProgressQuery(courseId), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetLearnersProgressQueryResult(IEnumerable<LearnerForGetCourseLearnersQuery> Learners);

public sealed record LearnerForGetCourseLearnersQuery
{
    public string Name { get; set; } = default!;
    public float Progress { get; set; }
    public DateTime? LastAccessTime { get; set; }
}

#endregion

public sealed record GetLearnersProgressQuery(string CourseId) : IRequest<Response>;

internal sealed class GeLearnersProgressQueryHandler : IRequestHandler<GetLearnersProgressQuery, Response>
{
    #region Constructor

    private readonly IRepository<Learner> _learnerRepo;
    private readonly IHashids _hashids;

    public GeLearnersProgressQueryHandler(IRepository<Learner> learnerRepo, IHashids hashids)
    {
        _learnerRepo = learnerRepo;
        _hashids = hashids;
    }

    #endregion

    public async Task<Response> Handle(GetLearnersProgressQuery query, CancellationToken cancellationToken)
    {
        if (!TryDecodeCourseId(query.CourseId, out int courseId))
            return NotFound("The course id is invalid.");

        List<Learner> learners = await GetLearnersWithProgressFromRepository(courseId, cancellationToken);

        List<LearnerForGetCourseLearnersQuery> learnersProgress = learners.Select(x => x.ToLearnerQuery()).ToList();

        return Ok(data: new GetLearnersProgressQueryResult(learnersProgress));
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId) =>
        _hashids.TryDecodeSingle(encodedCourseId, out courseId);

    private async Task<List<Learner>> GetLearnersWithProgressFromRepository(int courseId,
        CancellationToken cancellationToken)
    {
        return await _learnerRepo.ListAsync(new GetLearnersWithProgressSpec(courseId), cancellationToken);
    }


    private sealed class GetLearnersWithProgressSpec : Specification<Learner>
    {
        public GetLearnersWithProgressSpec(int courseId)
        {
            Query
                .Include(learner => learner.CoursesProgress.Where(x => x.CourseId == courseId))
                .AsNoTracking();
        }
    }

    #endregion
}

#region extensions

internal static class LearnerExtension
{
    public static LearnerForGetCourseLearnersQuery ToLearnerQuery(this Learner learner)
    {
        LearnerCourseProgress? courseProgress = learner.CoursesProgress.FirstOrDefault();

        return new LearnerForGetCourseLearnersQuery
        {
            Name = learner.FullName,
            Progress = courseProgress?.Progress ?? 0,
            LastAccessTime = courseProgress?.LastAccessTime
        };
    }
}

#endregion