using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public class DeleteCourseCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public DeleteCourseCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task DeleteTheCourseWhenCourseIdIsValid(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(instructor);
        var deleteCourseResponse = await client.DeleteAsync(courseId);

        deleteCourseResponse.EnsureSuccessStatusCode();

        var getByIdResponse = await client.GetAsync(courseId);

        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(InstructorUser instructor, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var deleteCourseResponse = await client.DeleteAsync(invalidCourseId);

        deleteCourseResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseDoesNotExist(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(instructor);

        await client.DeleteAsync(courseId);

        var deleteCourseResponse = await client.DeleteAsync(courseId);

        deleteCourseResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2)
    {
        var courseId = await _factory.CreateCourse(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var deleteCourseResponse = await client.DeleteAsync(courseId);

        deleteCourseResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task DeleteTheCourseWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var deleteCourseResponse = await client.DeleteAsync($"{courseId}");

        deleteCourseResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var deleteCourseResponse = await client.DeleteAsync($"{courseId}");

        deleteCourseResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var deleteCourseResponse = await client.DeleteAsync($"{courseId}");

        deleteCourseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}