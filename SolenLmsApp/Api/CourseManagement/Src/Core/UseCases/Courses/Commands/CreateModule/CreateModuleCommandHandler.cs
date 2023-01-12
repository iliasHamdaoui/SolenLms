using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateModule;

internal sealed class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, RequestResponse<string>>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<CreateModuleCommandHandler> _logger;

    public CreateModuleCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<CreateModuleCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task<RequestResponse<string>> Handle(CreateModuleCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            Course? course = await GetCourseFromRepository(courseId);
            if (course is null)
                return Error("The course does not exist.");

            Module createdModule = course.AddModule(command.ModuleTitle);

            await SaveCourseToRepository(course, createdModule);

            return Ok("The module has been created.", _hashids.Encode(createdModule.Id));
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the module.", ex);
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


    private async Task SaveCourseToRepository(Course course, Module createdModule)
    {
        await _repository.UpdateAsync(course);
        _logger.LogInformation("Module created. moduleId:{moduleId}, encodedModuleId:{encodedModuleId}",
            createdModule.Id,
            _hashids.Encode(createdModule.Id));
    }

    private RequestResponse<string> UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while creating the module. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private async Task<Course?> GetCourseFromRepository(int courseId)
    {
        Course? course = await _repository.FirstOrDefaultAsync(new GetCourseWithModulesSpec(courseId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private sealed class GetCourseWithModulesSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModulesSpec(int courseId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules);
        }
    }

    #endregion
}