using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.Learning.Core.
    UseCases.Courses.Queries.GetCourseById.GetCourseByIdQueryResult>;
using Response =
    Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.Learning.Core.UseCases.
        Courses.Queries.GetCourseById.GetCourseByIdQueryResult>;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseById;

internal sealed class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Response>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly ICurrentUser _currentUser;

    public GetCourseByIdQueryHandler(IRepository<Course> courseRepository, ICurrentUser currentUser)
    {
        _courseRepository = courseRepository;
        _currentUser = currentUser;
    }

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