using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
    CourseManagement.Core.UseCases.Courses.Queries.GetLectureById.GetLectureByIdQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;

using Response = RequestResponse<GetLectureByIdQueryResult>;

#nullable disable

internal sealed class GetLectureByIdQueryHandler : IRequestHandler<GetLectureByIdQuery, Response>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;

    public GetLectureByIdQueryHandler(IRepository<Course> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    public async Task<Response> Handle(GetLectureByIdQuery query, CancellationToken token)
    {
        if (!TryDecodeCourseId(query.CourseId, out int courseId))
            return NotFound("The course id is invalid.");

        if (!TryDecodeModuleId(query.ModuleId, out int moduleId))
            return NotFound("The module id is invalid.");

        if (!TryDecodeLectureId(query.LectureId, out int lectureId))
            return NotFound("The lecture id is invalid.");

        Course course = await LoadCourseWithModuleAndLectureFromRepository(courseId, moduleId, lectureId, token);
        if (course is null)
            return NotFound("The course does not exist.");

        Module module = GetTheLectureModule(course, moduleId);
        if (module is null)
            return NotFound("The module does not exist.");

        Lecture lecture = GetTheLecture(module, lectureId);
        if (lecture is null)
            return NotFound("The lecture does not exist.");

        return Ok(data: lecture.ToQueryResult());
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId) =>
        _hashids.TryDecodeSingle(encodedCourseId, out courseId);

    private bool TryDecodeModuleId(string encodedModuleId, out int moduleId) =>
        _hashids.TryDecodeSingle(encodedModuleId, out moduleId);


    private bool TryDecodeLectureId(string encodedLectureId, out int lectureId) =>
        _hashids.TryDecodeSingle(encodedLectureId, out lectureId);

    private static Module GetTheLectureModule(Course course, int moduleId)
    {
        return course.Modules.FirstOrDefault(x => x.Id == moduleId);
    }

    private static Lecture GetTheLecture(Module module, int lectureId)
    {
        return module.Lectures.FirstOrDefault(x => x.Id == lectureId);
    }

    private async Task<Course> LoadCourseWithModuleAndLectureFromRepository(int courseId, int moduleId, int lectureId,
        CancellationToken cancellationToken)
    {
        return await _repository.FirstOrDefaultAsync(new GetLectureByIdSpec(courseId, moduleId, lectureId),
            cancellationToken);
    }

    private sealed class GetLectureByIdSpec : SingleResultSpecification<Course>
    {
        public GetLectureByIdSpec(int courseId, int moduleId, int lectureId)
        {
            Query.Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures.Where(lecture => lecture.Id == lectureId))
                .AsNoTracking();
        }
    }

    #endregion
}