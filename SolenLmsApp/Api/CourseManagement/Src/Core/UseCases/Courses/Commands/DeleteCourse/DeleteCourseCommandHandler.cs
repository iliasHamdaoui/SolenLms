using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteCourse;

internal sealed class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, RequestResponse>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;
    private readonly IIntegratedEventsSender _eventsSender;

    public DeleteCourseCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<DeleteCourseCommandHandler> logger,
        IIntegratedEventsSender eventsSender)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    public async Task<RequestResponse> Handle(DeleteCourseCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("The course id is invalid");

            Course? course = await GetCourseFromRepository(courseId);
            if (course is null)
                return Error("The course does not exist.");

            await DeleteCourseFromRepository(course);

            await SendCourseDeletedEvent(course.OrganizationId, command.CourseId);

            return Ok("The course has been deleted.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while deleting the course.", ex);
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
        Course? course = await _repository.GetByIdAsync(courseId);
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task DeleteCourseFromRepository(Course course)
    {
        await _repository.DeleteAsync(course);

        _logger.LogInformation("Course deleted. courseId:{courseId}, encodedCourseId:{encodedCourseId}", course.Id,
            _hashids.Encode(course.Id));
    }

    private async Task SendCourseDeletedEvent(string organizationId, string encodedCourseId)
    {
        CourseDeleted courseDeletedEvent = new() { OrganizationId = organizationId, CourseId = encodedCourseId };

        await _eventsSender.SendEvent(courseDeletedEvent);
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while deleting the course. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}