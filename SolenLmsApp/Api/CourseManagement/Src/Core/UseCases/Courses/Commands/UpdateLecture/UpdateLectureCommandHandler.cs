using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecture;

internal sealed class UpdateLectureCommandHandler : IRequestHandler<UpdateLectureCommand, RequestResponse>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateLectureCommandHandler> _logger;

    public UpdateLectureCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<UpdateLectureCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(UpdateLectureCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("Invalid module id.");

            if (!TryDecodeLectureId(command.LectureId, out int lectureId))
                return Error("Invalid lecture id.");

            Course? course = await LoadCourseWithModuleAndLectureFromRepository(courseId, moduleId, lectureId);
            if (course is null)
                return Error("The course does not exist.");

            Module? module = GetModuleTheLectureBelongsTo(moduleId, course);
            if (module is null)
                return Error("The module does not exist.");

            Lecture? lectureToUpdate = GetTheLectureToUpdate(lectureId, module);
            if (lectureToUpdate is null)
                return Error("The lecture does not exist.");

            lectureToUpdate.UpdateTitle(command.LectureTitle);

            await SaveCourseToRepository(course, lectureToUpdate);

            return Ok("The lecture has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the lecture.", ex);
        }
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId)
    {
        if (_hashids.TryDecodeSingle(encodedCourseId, out courseId))
            return true;

        _logger.LogWarning("The encoded course id is invalid. encodedCourseId:{encodedCourseId}", encodedCourseId);
        return false;
    }

    private bool TryDecodeModuleId(string encodedModuleId, out int moduleId)
    {
        if (_hashids.TryDecodeSingle(encodedModuleId, out moduleId))
            return true;

        _logger.LogWarning("The encoded module id is invalid. encodedModuleId:{encodedModuleId}", encodedModuleId);
        return false;
    }

    private bool TryDecodeLectureId(string encodedLectureId, out int lectureId)
    {
        if (_hashids.TryDecodeSingle(encodedLectureId, out lectureId))
            return true;

        _logger.LogWarning("The encoded lecture id is invalid. encodedLectureId:{encodedLectureId}", encodedLectureId);
        return false;
    }

    private async Task<Course?> LoadCourseWithModuleAndLectureFromRepository(int courseId, int moduleId, int lectureId)
    {
        Course? course =
            await _repository.FirstOrDefaultAsync(new GetCourseWithModuleAndLectureSpec(courseId, moduleId, lectureId));

        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Module? GetModuleTheLectureBelongsTo(int moduleId, Course course)
    {
        Module? module = course.Modules.SingleOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private Lecture? GetTheLectureToUpdate(int lectureId, Module module)
    {
        Lecture? lecture = module.Lectures.SingleOrDefault(x => x.Id == lectureId);
        if (lecture is null)
            _logger.LogWarning("The lecture does not exist. lectureId:{lectureId}", lectureId);

        return lecture;
    }

    private async Task SaveCourseToRepository(Course course, Lecture lecture)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Lecture updated. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}",
            lecture.Id, _hashids.Encode(lecture.Id));
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while updating the lecture. message:{message}",
            exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private sealed class GetCourseWithModuleAndLectureSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleAndLectureSpec(int courseId, int moduleId, int lectureId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures.Where(lecture => lecture.Id == lectureId))
                .AsSplitQuery();
        }
    }

    #endregion
}