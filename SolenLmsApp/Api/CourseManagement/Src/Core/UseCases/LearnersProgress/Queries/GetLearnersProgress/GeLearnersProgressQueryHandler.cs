using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
    CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress.GetLearnersProgressQueryResult>;
using Response =
    Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
        CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress.GetLearnersProgressQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress;

internal sealed class GeLearnersProgressQueryHandler : IRequestHandler<GetLearnersProgressQuery, Response>
{
    private readonly IRepository<Learner> _learnerRepo;
    private readonly IHashids _hashids;

    public GeLearnersProgressQueryHandler(IRepository<Learner> learnerRepo, IHashids hashids)
    {
        _learnerRepo = learnerRepo;
        _hashids = hashids;
    }

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