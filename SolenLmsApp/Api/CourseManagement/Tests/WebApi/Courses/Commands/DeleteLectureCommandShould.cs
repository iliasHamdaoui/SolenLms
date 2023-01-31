using Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class DeleteLectureCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public DeleteLectureCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task DeleteTheLectureWhenLectureIdIsValid(string lectureTypeValue, InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.EnsureSuccessStatusCode();

        var getByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReorderRemainingLectures(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, firstLectureId) = await _factory.CreateLecture(instructor); // first lecture

        var (_, _, secondLectureId) =
            await _factory.CreateLecture(instructor, null, courseId, moduleId); // second lecture

        await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{firstLectureId}");

        var getSecondLectureByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                $"{courseId}/modules/{moduleId}/lectures/{secondLectureId}");

        getSecondLectureByIdResult.Data.Order.Should().Be(1);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenCourseIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var deleteLectureResponse =
            await client.DeleteAsync($"{invalidCourseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenModuleIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        string invalidModuleId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var deleteLectureResponse =
            await client.DeleteAsync($"{courseId}/modules/{invalidModuleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenLectureIdIsInvalid(string lectureTypeValue,
        InstructorUser instructor, string invalidLectureId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, _) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var deleteLectureResponse =
            await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{invalidLectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenCourseDoesNotExist(string lectureTypeValue,
        InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        await client.DeleteAsync($"{courseId}");

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenModuleDoesNotExist(string lectureTypeValue,
        InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenLectureDoesNotExist(string lectureTypeValue,
        InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnUnprocessableEntityWhenTheCourseDoestNotBelongToTheSameOrganizationAsTheInstructor(
        string lectureTypeValue, InstructorUser instructor1, InstructorUser instructor2)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor1, lectureTypeValue);

        var client = await _factory.CreateClientWithUser(instructor2);

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task DeleteTheLectureWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var deleteLectureResponse = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}