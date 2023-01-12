using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecturesOrders;

internal sealed class UpdateLecturesOrdersCommandHandler : IRequestHandler<UpdateLecturesOrdersCommand, RequestResponse>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateLecturesOrdersCommandHandler> _logger;

    public UpdateLecturesOrdersCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<UpdateLecturesOrdersCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(UpdateLecturesOrdersCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("The module id is invalid.");

            Course? course = await LoadCourseWithModuleAndLecturesFromRepository(courseId, moduleId);
            if (course is null)
                return Error("The course does not exists.");

            Module? module = GetLecturesModule(moduleId, course);
            if (module is null)
                return Error("The module does not exists.");

            UpdateLecturesOrders(module, command);

            await SaveCourseToRepository(course);

            return Ok("The lectures order has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the lectures orders.", ex);
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

    private async Task<Course?> LoadCourseWithModuleAndLecturesFromRepository(int courseId, int moduleId)
    {
        Course? course =
            await _repository.FirstOrDefaultAsync(new GetCourseWithModuleAndLecturesSpec(courseId, moduleId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Module? GetLecturesModule(int moduleId, Course course)
    {
        Module? module = course.Modules.FirstOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private void UpdateLecturesOrders(Module module, UpdateLecturesOrdersCommand command)
    {
        foreach (Lecture lecture in module.Lectures)
        {
            string? lectureId = _hashids.Encode(lecture.Id);

            int order = command.LecturesOrders.Any(x => x.LectureId == lectureId)
                ? command.LecturesOrders.First(x => x.LectureId == lectureId).Order
                : module.Order;

            lecture.UpdateOrder(order);
        }
    }

    private async Task SaveCourseToRepository(Course course)
    {
        await _repository.UpdateAsync(course);

        _logger.LogInformation("Lectures orders updated.");
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while reordering the lectures of the module. message:{message}",
            exception.Message);
        return Error(ResponseError.Unexpected, error);
    }


    private sealed class GetCourseWithModuleAndLecturesSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleAndLecturesSpec(int courseId, int moduleId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures);
        }
    }

    #endregion
}