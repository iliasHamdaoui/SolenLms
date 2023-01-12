using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateCourse;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class UpdateCourseCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public UpdateCourseCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheCourseInformationWhenTheUpdateCommandIsValid(InstructorUser instructor,
        UpdateCourseCommand command)
    {
        var now = DateTime.Now;
        _factory.DateTimeMoq.Setup(x => x.Now).Returns(now);
        var client = await _factory.CreateClientWithUser(instructor);
        var courseId = await _factory.CreateCourse(instructor);

        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");

        getCourseByIdResult.IsSuccess.Should().BeTrue();
        var course = getCourseByIdResult.Data;

        course.Title.Should().Be(command.CourseTitle);
        course.Description.Should().Be(command.CourseDescription);
        course.LastModifiedAt.Should().Be(now);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenCommandIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var courseId = await _factory.CreateCourse(instructor);

        var updateCourseResponse = await client.PutAsJsonAsync<UpdateCourseCommand>(courseId, null);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenTitleIsInvalid(InstructorUser instructor, UpdateCourseCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor);

        command.CourseTitle = null;
        var client = await _factory.CreateClientWithUser(instructor);
        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(InstructorUser instructor,
        UpdateCourseCommand command, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var updateCourseResponse = await client.PutAsJsonAsync(invalidCourseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseDoesNotExist(InstructorUser instructor,
        UpdateCourseCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor);
        var client = await _factory.CreateClientWithUser(instructor);
        await client.DeleteAsync(courseId);

        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2, UpdateCourseCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheCourseWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin,
        UpdateCourseCommand command)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner,
        UpdateCourseCommand command)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor,
        UpdateCourseCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var updateCourseResponse = await client.PutAsJsonAsync(courseId, command);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}