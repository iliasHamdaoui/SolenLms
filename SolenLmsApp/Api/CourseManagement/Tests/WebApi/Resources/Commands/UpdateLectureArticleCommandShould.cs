using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UpdateLectureArticle;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Resources.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class UpdateLectureArticleCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public UpdateLectureArticleCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.ResourcesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheArticleContentWhenTheCommandIsValid(InstructorUser instructor,
        UpdateLectureArticleCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (_, resourceId, _, _) = await CreateArticleLecture(instructor);

        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getLectureArticleQueryResponse =
            await client.GetFromJsonAsync<RequestResponse<string>>($"article/{resourceId}");

        getLectureArticleQueryResponse.Data.Should().Be(command.Content);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenResourceIdIsInvalid(InstructorUser instructor,
        UpdateLectureArticleCommand command, string invalidResourceId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{invalidResourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenResourceDoesNotExist(InstructorUser instructor,
        UpdateLectureArticleCommand command)
    {
        var (lectureId, resourceId, moduleId, courseId) = await CreateArticleLecture(instructor);

        await _factory.DeleteLecture(instructor, courseId, moduleId, lectureId);

        var client = await _factory.CreateClientWithUser(instructor);
        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessedEntityWhenTheResourceDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2, UpdateLectureArticleCommand command)
    {
        var (_, resourceId, _, _) = await CreateArticleLecture(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);
        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheLectureContentFormatIsIncorrect(InstructorUser instructor,
        UpdateLectureArticleCommand command)
    {
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var client = await _factory.CreateClientWithUser(instructor);

        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task UpdateTheArticleContentWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin,
        UpdateLectureArticleCommand command)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (_, resourceId, _, _) = await CreateArticleLecture(instructor);

        var client = await _factory.CreateClientWithUser(admin);
        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner,
        UpdateLectureArticleCommand command)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (_, resourceId, _, _) = await CreateArticleLecture(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor,
        UpdateLectureArticleCommand command)
    {
        var (_, resourceId, _, _) = await CreateArticleLecture(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var updateLectureArticleCommandResponse = await client.PutAsJsonAsync($"{resourceId}/article", command);

        updateLectureArticleCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #region private methods

    private async Task<(string lectureId, string resourceId, string moduleId, string courseId)> CreateArticleLecture(
        InstructorUser instructor)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, "Article");
        var lecture = await _factory.GetLecture(instructor, courseId, moduleId, lectureId);

        return (lectureId, lecture.ResourceId, moduleId, courseId);
    }

    private async Task<(string lectureId, string resourceId, string moduleId, string courseId)> CreateVideoLecture(
        InstructorUser instructor)
    {
        var (courseId, moduleId, lectureId) = await _factory.CreateLecture(instructor, "Video");
        var lecture = await _factory.GetLecture(instructor, courseId, moduleId, lectureId);

        return (lectureId, lecture.ResourceId, moduleId, courseId);
    }

    #endregion
}