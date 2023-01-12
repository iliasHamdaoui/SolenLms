using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public class UnpublishCourseCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public UnpublishCourseCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task UnpublishTheCourseWhenTheCommandIsValid(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(instructor);

        var unpublishCourseCommandResponse = await client.PutAsync($"{courseId}/unpublish", null);

        unpublishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");

        var unpublishedCourse = getCourseByIdResult.Data;

        unpublishedCourse.IsPublished.Should().BeFalse();
        unpublishedCourse.PublicationDate.Should().BeNull();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2)
    {
        var courseId = await _factory.CreateCourse(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var unpublishCourseCommandResponse = await client.PutAsync($"{courseId}/unpublish", null);

        unpublishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task UnpublishTheCourseWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var unpublishCourseCommandResponse = await client.PutAsync($"{courseId}/unpublish", null);

        unpublishCourseCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var unpublishCourseCommandResponse = await client.PutAsync($"{courseId}/unpublish", null);

        unpublishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var unpublishCourseCommandResponse = await client.PutAsync($"{courseId}/unpublish", null);

        unpublishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}