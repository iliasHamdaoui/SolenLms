using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModule;

internal class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, RequestResponse>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateModuleCommandHandler> _logger;

    public UpdateModuleCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<UpdateModuleCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(UpdateModuleCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("Invalid module id.");

            Course? course = await LoadCourseWithTheModuleFromRepository(courseId, moduleId);
            if (course is null)
                return Error("The course does not exist.");

            Module? moduleToUpdate = GetModuleToUpdate(moduleId, course);
            if (moduleToUpdate is null)
                return Error("The module does not exist.");

            moduleToUpdate.UpdateTitle(command.ModuleTitle);

            await SaveCourseToRepository(course, moduleId, command.ModuleId);

            return Ok("The module has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the module.", ex);
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

    private async Task<Course?> LoadCourseWithTheModuleFromRepository(int courseId, int moduleId)
    {
        Course? course =
            await _repository.FirstOrDefaultAsync(new GetCourseWithModuleSpec(courseId, moduleId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Module? GetModuleToUpdate(int moduleId, Course course)
    {
        Module? module = course.Modules.FirstOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private async Task SaveCourseToRepository(Course course, int moduleId, string encodedModuleId)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Module updated. moduleId:{moduleId}, encodedModuleId:{encodedModuleId}",
            moduleId, encodedModuleId);
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while updating the module. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }


    private sealed class GetCourseWithModuleSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleSpec(int courseId, int moduleId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId));
        }
    }

    #endregion
}