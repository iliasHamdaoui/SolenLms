using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateCourse;

internal sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, RequestResponse<string>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(IRepository<Course> courseRepository, IHashids hashids, ICurrentUser currentUser,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<RequestResponse<string>> Handle(CreateCourseCommand command, CancellationToken _)
    {
        try
        {
            Course newCourse = CreateNewCourse(command.CourseTitle);

            newCourse.UpdateDescription(command.CourseDescription);

            SetCurrentUserAsInstructorByDefault(newCourse);

            await AddNewCourseToRepository(newCourse);

            return Ok("The course has been created", _hashids.Encode(newCourse.Id));
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the course.", ex);
        }
    }


    #region private methods

    private Course CreateNewCourse(string title)
    {
        return new Course(_currentUser.OrganizationId, title);
    }

    private async Task AddNewCourseToRepository(Course newCourse)
    {
        await _courseRepository.AddAsync(newCourse);

        _logger.LogInformation("Course created. courseId:{courseId}, encodedCourseId:{encodedCourseId}", newCourse.Id,
            _hashids.Encode(newCourse.Id));
    }

    private void SetCurrentUserAsInstructorByDefault(Course newCourse)
    {
        newCourse.UpdateInstructorId(_currentUser.UserId);
    }

    private RequestResponse<string> UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while creating the course. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}