using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UnpublishCourse;

internal sealed class UnpublishCourseCommandHandler : IRequestHandler<UnpublishCourseCommand, RequestResponse>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;
    private readonly ILogger<UnpublishCourseCommandHandler> _logger;
    private readonly IIntegratedEventsSender _eventsSender;

    public UnpublishCourseCommandHandler(IRepository<Course> courseRepository, IHashids hashids,
        ILogger<UnpublishCourseCommandHandler> logger, IIntegratedEventsSender eventsSender)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    public async Task<RequestResponse> Handle(UnpublishCourseCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            Course? courseToUnpublish = await GetCourseFromRepository(courseId);
            if (courseToUnpublish is null)
                return Error("The course does not exist.");

            courseToUnpublish.ResetPublicationDate();

            await SaveCourseToRepository(courseToUnpublish);

            await SendCourseUnpublishedEvent(courseId, command.CourseId);

            return Ok("The training course has been unpublished.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while unpublishing the course.", ex);
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
    }

    private async Task SendCourseUnpublishedEvent(int courseId, string encodedCourseId)
    {
        await _eventsSender.SendEvent(new CourseUnpublished(encodedCourseId));

        _logger.LogInformation("Course unpublished. courseId:{courseId}, encodedCourseId:{encodedCourseId}", courseId,
            encodedCourseId);
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while unpublishing the course. message:{message}",
            exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}