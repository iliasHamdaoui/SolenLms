using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.PublishCourse;

internal sealed class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand, RequestResponse>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;
    private readonly IDateTime _dateTime;
    private readonly ILogger<PublishCourseCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public PublishCourseCommandHandler(IRepository<Course> courseRepository, IHashids hashids, IDateTime dateTime,
        ILogger<PublishCourseCommandHandler> logger, IIntegrationEventsSender eventsSender)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
        _dateTime = dateTime;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    public async Task<RequestResponse> Handle(PublishCourseCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            Course? courseToPublish = await LoadCourseWithModulesAndLecturesFromRepository(courseId);
            if (courseToPublish is null)
                return RequestResponse<string>.Error("The course does not exist.");

            courseToPublish.SetPublicationDate(_dateTime.Now);

            await SaveCourseToRepository(courseToPublish);

            await SendCoursePublishedEvent(courseToPublish);

            return Ok("The training course has been published.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while publishing the course.", ex);
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

    private async Task<Course?> LoadCourseWithModulesAndLecturesFromRepository(int courseId)
    {
        Course? course = await _courseRepository.FirstOrDefaultAsync(new GetCompleteCourseInfoByIdSpec(courseId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private Task SaveCourseToRepository(Course courseToPublish)
    {
        return _courseRepository.UpdateAsync(courseToPublish);
    }

    private async Task SendCoursePublishedEvent(Course publishedCourse)
    {
        await _eventsSender.SendEvent(publishedCourse.ToCoursePublishedEvent(_hashids));

        _logger.LogInformation("Course published. courseId:{courseId}, encodedCourseId:{encodedCourseId}",
            publishedCourse.Id, _hashids.Encode(publishedCourse.Id));
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while publishing the course. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }


    private sealed class GetCompleteCourseInfoByIdSpec : SingleResultSpecification<Course>
    {
        public GetCompleteCourseInfoByIdSpec(int courseId)
        {
            Query
                .Where(course => course.Id == courseId)
                .Include(course => course.Categories)
                .Include(course => course.Modules)
                .ThenInclude(module => module.Lectures)
                .AsSplitQuery();
        }
    }

    #endregion
}