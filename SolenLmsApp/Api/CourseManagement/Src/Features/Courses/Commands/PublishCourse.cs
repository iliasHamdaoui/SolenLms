using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.PublishCourse;

#region Web API

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// publish a training course
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content</returns>
    [HttpPut("{courseId}/publish")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> PublishCourse(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new PublishCourseCommand(courseId), cancellationToken));
    }
}

#endregion

public sealed record PublishCourseCommand(string CourseId) : IRequest<RequestResponse>;

internal sealed class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand, RequestResponse>
{
    #region Constructor

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

    #endregion

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

#region extensions

internal static class CourseExtensions
{
    public static CoursePublished ToCoursePublishedEvent(this Course course, IHashids hashids)
    {
        PublishedCourse publishedCourse = new()
        {
            OrganizationId = course.OrganizationId,
            CourseId = hashids.Encode(course.Id),
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            Duration = course.Modules.Select(x => x.ToPublishedCourseModule(hashids)).ToList().Sum(x => x.Duration),
            PublicationDate = course.PublicationDate!.Value,
            Modules = course.Modules.Select(x => x.ToPublishedCourseModule(hashids)).ToList(),
            CategoriesId = course.Categories.Select(x => x.CategoryId).ToList()
        };

        return new CoursePublished(publishedCourse);
    }

    private static PublishedCourseLecture ToPublishedCourseLecture(this Lecture lecture, IHashids hashids)
    {
        return new PublishedCourseLecture
        {
            Id = hashids.Encode(lecture.Id),
            Title = lecture.Title,
            LectureType = lecture.Type,
            Duration = lecture.Duration,
            Order = lecture.Order,
            ResourceId = lecture.ResourceId
        };
    }

    private static PublishedCourseModule ToPublishedCourseModule(this Module module, IHashids hashids)
    {
        List<PublishedCourseLecture> publishedLectures =
            module.Lectures.Select(x => x.ToPublishedCourseLecture(hashids)).ToList();
        int moduleDuration = publishedLectures.Sum(x => x.Duration);

        return new PublishedCourseModule
        {
            Id = hashids.Encode(module.Id),
            Title = module.Title,
            Duration = moduleDuration,
            Order = module.Order,
            Lectures = publishedLectures
        };
    }
}

#endregion