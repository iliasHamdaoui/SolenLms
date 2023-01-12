using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public class DeleteModuleCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public DeleteModuleCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task DeleteTheModuleWhenModuleIdIsValid(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.EnsureSuccessStatusCode();

        var getByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}");

        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReorderRemainingModules(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, firstModuleId) = await _factory.CreateModule(instructor); // first module

        var (_, secondModuleId) = await _factory.CreateModule(instructor, courseId); // second module

        await client.DeleteAsync($"{courseId}/modules/{firstModuleId}");

        var getSecondModuleByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetModuleByIdQueryResult>>(
                $"{courseId}/modules/{secondModuleId}");

        getSecondModuleByIdResult.Data.Order.Should().Be(1);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(InstructorUser instructor, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId) = await _factory.CreateModule(instructor);

        var deleteModuleResponse = await client.DeleteAsync($"{invalidCourseId}/modules/{moduleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenModuleIdIsInvalid(InstructorUser instructor, string invalidModuleId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _) = await _factory.CreateModule(instructor);

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{invalidModuleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseDoesNotExist(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        await client.DeleteAsync(courseId);

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenModuleDoesNotExist(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2)
    {
        await _factory.CreateClientWithUser(instructor1);

        var (courseId, moduleId) = await _factory.CreateModule(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task DeleteTheModuleWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var deleteModuleResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        deleteModuleResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}