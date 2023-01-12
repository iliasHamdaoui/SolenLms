using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateLecture;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class CreateLectureCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public CreateLectureCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task CreateTheExpectedLectureWhenTheCommandIsValid(string lectureTypeValue, InstructorUser instructor,
        CreateLectureCommand command)
    {
        var (isSuccess, lectureType) = Enumeration.FromValue<LectureType>(lectureTypeValue);
        isSuccess.Should().BeTrue();
        command.LectureType = lectureType.Value;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getLectureByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                $"{createLectureCommandResponse.Headers.Location}");

        getLectureByIdResult.IsSuccess.Should().BeTrue();

        var createdLecture = getLectureByIdResult.Data;

        createdLecture.Title.Should().Be(command.LectureTitle);
        createdLecture.Type.Should().Be(lectureType.Value);
        createdLecture.MediaType.Should().Be(lectureType.MediaType?.Value);
        createdLecture.Duration.Should().Be(0);
        createdLecture.Order.Should().Be(1);
    }

    [Theory]
    [InjectTestUser]
    public async Task CreateTheExpectedLectureWithTheCorrectOrder(InstructorUser instructor)
    {
        var (courseId, moduleId, _) = await _factory.CreateLecture(instructor); // first lecture

        var (_, _, secondLectureId) =
            await _factory.CreateLecture(instructor, null, courseId, moduleId); // second lecture

        var client = await _factory.CreateClientWithUser(instructor);

        var getSecondLectureByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                $"{courseId}/modules/{moduleId}/lectures/{secondLectureId}");

        getSecondLectureByIdResult.Data.Order.Should().Be(2);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenCommandIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync<CreateLectureCommand>($"{courseId}/modules/{moduleId}/lectures", null);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnBadRequestWhenTheTitleIsNull(string lectureTypeValue, InstructorUser instructor,
        CreateLectureCommand command)
    {
        command.LectureType = lectureTypeValue;
        command.LectureTitle = null;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenTheLectureTypeIsNull(InstructorUser instructor, CreateLectureCommand command)
    {
        command.LectureType = null;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        CreateLectureCommand command, string invalidCourseId)
    {
        command.LectureType = lectureTypeValue;

        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{invalidCourseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenModuleIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        CreateLectureCommand command, string invalidModuleId)
    {
        command.LectureType = lectureTypeValue;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{invalidModuleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenLectureTypeIsInvalid(InstructorUser instructor,
        CreateLectureCommand command, string invalidLectureType)
    {
        command.LectureType = invalidLectureType;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoesNotExist(string lectureTypeValue,
        InstructorUser instructor, CreateLectureCommand command)
    {
        command.LectureType = lectureTypeValue;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);
        await client.DeleteAsync(courseId);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenTheModuleDoesNotExist(string lectureTypeValue,
        InstructorUser instructor, CreateLectureCommand command)
    {
        command.LectureType = lectureTypeValue;

        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId) = await _factory.CreateModule(instructor);
        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }


    [Theory]
    [InjectTestUser]
    public async Task CreateTheLectureWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin,
        CreateLectureCommand command)
    {
        admin.OrganizationId = instructor.OrganizationId;
        command.LectureType = "Article";
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner,
        CreateLectureCommand command)
    {
        learner.OrganizationId = instructor.OrganizationId;
        command.LectureType = "Article";
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor,
        CreateLectureCommand command)
    {
        command.LectureType = "Article";
        var (courseId, moduleId) = await _factory.CreateModule(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var createLectureCommandResponse =
            await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", command);

        createLectureCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}