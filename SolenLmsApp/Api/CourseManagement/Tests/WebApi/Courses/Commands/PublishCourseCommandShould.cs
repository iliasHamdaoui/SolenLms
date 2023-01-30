using Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public class PublishCourseCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public PublishCourseCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task PublishTheCourseWhenTheCommandIsValid(InstructorUser instructor)
    {
        var now = DateTime.Now;
        _factory.DateTimeMoq.Setup(x => x.Now).Returns(now);

        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(instructor);

        var publishCourseCommandResponse = await client.PutAsync($"{courseId}/publish", null);

        publishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");

        var publishedCourse = getCourseByIdResult.Data;

        publishedCourse.IsPublished.Should().BeTrue();
        publishedCourse.PublicationDate.Should().Be(now);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2)
    {
        var courseId = await _factory.CreateCourse(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var publishCourseCommandResponse = await client.PutAsync($"{courseId}/publish", null);

        publishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task PublishTheCourseWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var publishCourseCommandResponse = await client.PutAsync($"{courseId}/publish", null);

        publishCourseCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var publishCourseCommandResponse = await client.PutAsync($"{courseId}/publish", null);

        publishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var publishCourseCommandResponse = await client.PutAsync($"{courseId}/publish", null);

        publishCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}