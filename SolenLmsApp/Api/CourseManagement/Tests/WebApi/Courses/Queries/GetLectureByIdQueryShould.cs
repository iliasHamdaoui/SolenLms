using Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Queries;

[Collection("CourseManagementWebApplicationFactory")]
public class GetLectureByIdQueryShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public GetLectureByIdQueryShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnExpectedLectureWhenValidLectureId(string lectureTypeValue, InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var getLectureByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                $"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        getLectureByIdResult.Should().NotBeNull();
        getLectureByIdResult?.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundTheCourseIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (_, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var getLectureByIdResponse =
            await client.GetAsync($"{invalidCourseId}/modules/{moduleId}/lectures/{lectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundWhenTheModuleIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        string invalidModuleId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, _, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var getLectureByIdResponse =
            await client.GetAsync($"{courseId}/modules/{invalidModuleId}/lectures/{lectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundWhenTheLectureIdIsInvalid(string lectureTypeValue, InstructorUser instructor,
        string invalidLectureId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, _) = await _factory.CreateLecture(instructor, lectureTypeValue);

        var getLectureByIdResponse =
            await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{invalidLectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundWhenTheCourseHasBeenDeleted(string lectureTypeValue, InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);
        await client.DeleteAsync(courseId);

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundWhenTheModuleHasBeenDeleted(string lectureTypeValue, InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);
        await client.DeleteAsync($"{courseId}/modules/{moduleId}");

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundWhenTheLectureHasBeenDeleted(string lectureTypeValue, InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, lectureTypeValue);
        await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Theory]
    [InlineInjectTestUser("Article")]
    [InlineInjectTestUser("Video")]
    public async Task ReturnNotFoundWhenCourseBelongsToAnOtherOrganization(string lectureTypeValue,
        InstructorUser instructor1, InstructorUser instructor2)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor1, lectureTypeValue);

        var client = await _factory.CreateClientWithUser(instructor2);

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnTheLectureWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");
        getLectureByIdResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");
        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var getLectureByIdResponse = await client.GetAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");
        getLectureByIdResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}