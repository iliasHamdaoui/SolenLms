using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateModule;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class CreateModuleCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public CreateModuleCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task CreateTheExpectedModuleWhenTheCommandIsValid(InstructorUser instructor,
        CreateModuleCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(instructor);
        var createModuleCommandResponse = await client.PostAsJsonAsync($"{courseId}/modules", command);

        createModuleCommandResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getModuleByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetModuleByIdQueryResult>>(
                $"{createModuleCommandResponse.Headers.Location}");

        getModuleByIdResult.IsSuccess.Should().BeTrue();

        var createdModule = getModuleByIdResult.Data;

        createdModule.Title.Should().Be(command.ModuleTitle);
        createdModule.Duration.Should().Be(0);
        createdModule.Lectures.Should().BeEmpty();
    }

    [Theory]
    [InjectTestUser]
    public async Task CreateTheExpectedModuleWithTheCorrectOrder(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _) = await _factory.CreateModule(instructor); // first module

        var (_, secondModuleId) = await _factory.CreateModule(instructor, courseId); // second module

        var getSecondModuleByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetModuleByIdQueryResult>>(
                $"{courseId}/modules/{secondModuleId}");

        getSecondModuleByIdResult.Data.Order.Should().Be(2);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenCommandIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var courseId = await _factory.CreateCourse(instructor);

        var createModuleCommandResponse =
            await client.PostAsJsonAsync<CreateModuleCommand>($"{courseId}/modules", null);

        createModuleCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenTheTitleIsInvalid(InstructorUser instructor, CreateModuleCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var courseId = await _factory.CreateCourse(instructor);
        command.ModuleTitle = null;

        var createModuleCommandResponse = await client.PostAsJsonAsync($"{courseId}/modules", command);

        createModuleCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(InstructorUser instructor,
        CreateModuleCommand command, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var createModuleResponse = await client.PostAsJsonAsync($"{invalidCourseId}/modules", command);

        createModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseDoesNotExist(InstructorUser instructor,
        CreateModuleCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(instructor);
        await client.DeleteAsync(courseId);

        var createModuleResponse = await client.PostAsJsonAsync($"{courseId}/modules", command);

        createModuleResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task CreateTheModuleWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin,
        CreateModuleCommand command)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var createModuleCommandResponse = await client.PostAsJsonAsync($"{courseId}/modules", command);

        createModuleCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner,
        CreateModuleCommand command)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var createModuleCommandResponse = await client.PostAsJsonAsync($"{courseId}/modules", command);

        createModuleCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor,
        CreateModuleCommand command)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var createModuleCommandResponse = await client.PostAsJsonAsync($"{courseId}/modules", command);

        createModuleCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}