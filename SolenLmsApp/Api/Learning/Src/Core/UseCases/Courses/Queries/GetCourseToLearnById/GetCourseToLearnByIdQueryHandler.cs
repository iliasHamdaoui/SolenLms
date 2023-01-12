using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.Learning.Core.
    UseCases.Courses.Queries.GetCourseToLearnById.GetCourseToLearnByIdQueryResult>;
using Response =
    Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.Learning.Core.UseCases.
        Courses.Queries.GetCourseToLearnById.GetCourseToLearnByIdQueryResult>;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseToLearnById;

internal sealed class GetCourseToLearnByIdQueryHandler : IRequestHandler<GetCourseToLearnByIdQuery, Response>
{
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

    public async Task<Response> Handle(GetCourseToLearnByIdQuery query, CancellationToken cancellationToken)
    {
        Course? course = await GetCourseFromRepository(query.CourseId, cancellationToken);

        if (course is null)
            return NotFound("The course does not exist.");

        LearnerCourseAccess? lastAccessedLecture = await GetLastAccessedCourseFromRepository(cancellationToken, course);

        return Ok(data: course.ToQueryResult(lastAccessedLecture));
    }

    #region private methods

    private async Task<Course?> GetCourseFromRepository(string courseId, CancellationToken cancellationToken)
    {
        return await _courseRepository.FirstOrDefaultAsync(new GetCourseByIdSpec(courseId, _currentUser.UserId),
            cancellationToken);
    }

    private async Task<LearnerCourseAccess?> GetLastAccessedCourseFromRepository(CancellationToken cancellationToken,
        Course course)
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