using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModule;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public class UpdateModuleCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public UpdateModuleCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheModuleInformationWhenTheUpdateCommandIsValid(InstructorUser instructor,
        UpdateModuleCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getModuleByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetModuleByIdQueryResult>>($"{courseId}/modules/{moduleId}");

        var module = getModuleByIdResult.Data;

        module.Title.Should().Be(command.ModuleTitle);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenCommandIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var updateCourseResponse =
            await client.PutAsJsonAsync<UpdateModuleCommand>($"{courseId}/modules/{moduleId}", null);

        updateCourseResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenTitleIsInvalid(InstructorUser instructor, UpdateModuleCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        command.ModuleTitle = null;

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(InstructorUser instructor,
        UpdateModuleCommand command, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId) = await _factory.CreateModule(instructor);

        var updateModuleResponse = await client.PutAsJsonAsync($"{invalidCourseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenModuleIdIsInvalid(InstructorUser instructor,
        UpdateModuleCommand command, string invalidModuleId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _) = await _factory.CreateModule(instructor);

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{invalidModuleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseHasBeenDeleted(InstructorUser instructor,
        UpdateModuleCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (courseId, moduleId) = await _factory.CreateModule(instructor);
        await client.DeleteAsync(courseId);

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenModuleHasBeenDeleted(InstructorUser instructor,
        UpdateModuleCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (courseId, moduleId) = await _factory.CreateModule(instructor);
        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2, UpdateModuleCommand command)
    {
        var (courseId, moduleId) = await _factory.CreateModule(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheModuleWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin,
        UpdateModuleCommand command)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner,
        UpdateModuleCommand command)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor,
        UpdateModuleCommand command)
    {
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var updateModuleResponse = await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}", command);

        updateModuleResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}