using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecture;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class UpdateLectureCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public UpdateLectureCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheLectureInformationWhenTheUpdateCommandIsValid(InstructorUser instructor,
        UpdateLectureCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getLectureByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                $"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        var lecture = getLectureByIdResult.Data;

        lecture.Title.Should().Be(command.LectureTitle);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenCommandIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var updateLectureResponse =
            await client.PutAsJsonAsync<UpdateLectureCommand>($"{courseId}/modules/{moduleId}/lectures/{lectureId}",
                null);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenTitleIsInvalid(InstructorUser instructor, UpdateLectureCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);
        command.LectureTitle = null;

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(InstructorUser instructor,
        UpdateLectureCommand command, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{invalidCourseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenModuleIdIsInvalid(InstructorUser instructor,
        UpdateLectureCommand command, string invalidModuleId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{invalidModuleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenLectureIdIsInvalid(InstructorUser instructor,
        UpdateLectureCommand command, string invalidLectureId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{invalidLectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseHasBeenDeleted(InstructorUser instructor,
        UpdateLectureCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        await client.DeleteAsync($"{courseId}");

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheModuleHasBeenDeleted(InstructorUser instructor,
        UpdateLectureCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheLectureHasBeenDeleted(InstructorUser instructor,
        UpdateLectureCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2, UpdateLectureCommand command)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheLectureWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin,
        UpdateLectureCommand command)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner,
        UpdateLectureCommand command)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor,
        UpdateLectureCommand command)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var updateLectureResponse =
            await client.PutAsJsonAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}", command);

        updateLectureResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}