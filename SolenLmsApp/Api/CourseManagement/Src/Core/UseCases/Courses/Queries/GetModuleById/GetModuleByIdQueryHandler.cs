using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
    CourseManagement.Core.UseCases.Courses.Queries.GetModuleById.GetModuleByIdQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;

using Response = RequestResponse<GetModuleByIdQueryResult>;

internal sealed class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery, Response>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;

    public GetModuleByIdQueryHandler(IRepository<Course> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

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