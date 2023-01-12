using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateCourse;

internal sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, RequestResponse>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(IRepository<Course> courseRepository, IHashids hashids,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(UpdateCourseCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            Course? courseToUpdate = await GetCourseFromRepository(courseId);
            if (courseToUpdate is null)
                return Error("The course does not exist.");

            courseToUpdate.UpdateTitle(command.CourseTitle);
            courseToUpdate.UpdateDescription(command.CourseDescription);

            await SaveCourseToRepository(courseToUpdate);

            return Ok("The training course has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the course.", ex);
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

    private async Task<Course?> GetCourseFromRepository(int courseId)
    {
        Course? course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task SaveCourseToRepository(Course courseToUnpublish)
    {
        await _courseRepository.UpdateAsync(courseToUnpublish);

        _logger.LogInformation("Course updated. courseId:{courseId}, encodedCourseId:{encodedCourseId}",
            courseToUnpublish.Id, _hashids.Encode(courseToUnpublish.Id));
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while updating the course. message:{message}",
            exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}