using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;
using System.IO;
using System.Net.Http.Headers;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Resources.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class UploadLectureVideoCommandShould
{
    private const string _testFilesFolder = "WebApi/Resources/TestFiles/";
    private readonly CourseManagementWebApplicationFactory _factory;

    public UploadLectureVideoCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.ResourcesBaseUrl;
        _factory = factory;
    }


    [Theory]
    [InjectTestUser]
    public async Task UploadTheVideoWhenTheVideoFileIsValid(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenFileContentIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var videoStreamContent = await GetValidVideoContent();
        videoStreamContent.Headers.ContentType = null;

        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenFileContentTypeIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        MultipartFormDataContent form = new();

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenResourceIdIsInvalid(InstructorUser instructor,
        string invalidResourceId)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{invalidResourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenResourceDoesNotExist(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (lectureId, resourceId, moduleId, courseId) = await CreateVideoLecture(instructor);

        await _factory.DeleteLecture(instructor, courseId, moduleId, lectureId);

        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheResourceDoestNotBelongToTheSameOrganizationAsTheInstructor(
        InstructorUser instructor1, InstructorUser instructor2)
    {
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor1);

        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var client = await _factory.CreateClientWithUser(instructor2);

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnprocessableEntityWhenTheFileFormatIsIncorrect(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var videoStreamContent = await GetInvalidVideoFormatContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "invalid_video_format.mov" } };

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }


    [Theory]
    [InjectTestUser]
    public async Task UploadTheVideoWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var client = await _factory.CreateClientWithUser(admin);

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);

        uploadLectureVideoCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var client = await _factory.CreateClientWithUser(learner);

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);
        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var (_, resourceId, _, _) = await CreateVideoLecture(instructor);

        var videoStreamContent = await GetValidVideoContent();
        MultipartFormDataContent form = new() { { videoStreamContent, "file", "valid_video.mp4" } };

        var client = _factory.CreateAnonymousUserClient();

        var uploadLectureVideoCommandResponse = await client.PutAsync($"{resourceId}/video", form);
        uploadLectureVideoCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    #region private methods

    private async Task<StreamContent> GetValidVideoContent()
    {
        var memoryStream = new MemoryStream();
        var fileStream = File.OpenRead($"{_testFilesFolder}/valid_video.mp4");
        await fileStream.CopyToAsync(memoryStream);
        fileStream.Close();

        var streamContent = new StreamContent(memoryStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");

        return streamContent;
    }

    private async Task<StreamContent> GetInvalidVideoFormatContent()
    {
        var memoryStream = new MemoryStream();
        var fileStream = File.OpenRead($"{_testFilesFolder}/invalid_video_format.mov");
        await fileStream.CopyToAsync(memoryStream);
        fileStream.Close();

        var streamContent = new StreamContent(memoryStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("video/quicktime");

        return streamContent;
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