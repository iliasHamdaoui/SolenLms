using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
    CourseManagement.Core.UseCases.Courses.Queries.GetCourseById.GetCourseByIdQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;

internal sealed class
    GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, RequestResponse<GetCourseByIdQueryResult>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;

    public GetCourseByIdQueryHandler(IRepository<Course> courseRepository, IHashids hashids)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
    }

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