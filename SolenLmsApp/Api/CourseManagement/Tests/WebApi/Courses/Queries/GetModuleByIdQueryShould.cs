using Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetModuleById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Queries;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class GetModuleByIdQueryShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public GetModuleByIdQueryShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnExpectedModuleWhenValidCourseId(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var getModuleByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetModuleByIdQueryResult>>($"{courseId}/modules/{moduleId}");

        getModuleByIdResult.Should().NotBeNull();
        getModuleByIdResult?.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundTheCourseIdIsInvalid(InstructorUser instructor, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId) = await _factory.CreateModule(instructor);

        var getModuleByIdResponse = await client.GetAsync($"{invalidCourseId}/modules/{moduleId}");

        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundTheModuleIdIsInvalid(InstructorUser instructor, string invalidModuleId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _) = await _factory.CreateModule(instructor);

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{invalidModuleId}");

        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheModuleHasBeenDeleted(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");

        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheCourseHasBeenDeleted(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        await client.DeleteAsync($"{courseId}");

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");

        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFounddWhenCourseBelongsToAnOtherOrganization(InstructorUser instructor1,
        InstructorUser instructor2)
    {
        var client = await _factory.CreateClientWithUser(instructor1);

        var (courseId, moduleId) = await _factory.CreateModule(instructor1);

        client = await _factory.CreateClientWithUser(instructor2);

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");

        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnTheModuleWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");
        getModuleByIdResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");
        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var getModuleByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");
        getModuleByIdResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}