using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UpdateLectureArticle;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;
using System.Text;


namespace Imanys.SolenLms.Application.Learning.Tests;

public sealed class LearningWebApplicationFactory : CustomSharedWebApplicationFactory
{
    public async Task<GetCourseByIdQueryResult> CreateCourse(InstructorUser instructor)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var response = await client.PostAsJsonAsync("", GetValidCourseCreationCommand());

        var courseId = (await response.Content.ReadFromJsonAsync<RequestResponse<string>>())?.Data;
        var numberOfModules = new Random().Next(2, 4);
        for (int i = 0; i < numberOfModules; i++)
        {
            var (_, moduleId) = await CreateModule(instructor, courseId);
            var numberOfLectures = new Random().Next(1, 4);

            for (int j = 0; j < numberOfLectures; j++)
            {
                var (_, _, lectureId) = await CreateLecture(instructor, courseId, moduleId);

                var getLectureByIdResult = await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

                await CreateLectureResourceContent(instructor, getLectureByIdResult.Data.ResourceId);
            }
        }

        var getCourseByIdResult = await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");
        return getCourseByIdResult.Data;
    }

    public async Task PublishCourse(InstructorUser instructor, string courseId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var publicationResponse = await client.PutAsync($"{courseId}/publish", null);
        publicationResponse.EnsureSuccessStatusCode();
    }

    public async Task UnpublishCourse(InstructorUser instructor, string courseId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var unpublicationResponse = await client.PutAsync($"{courseId}/unpublish", null);
        unpublicationResponse.EnsureSuccessStatusCode();
    }

    public async Task DeleteCourse(InstructorUser instructor, string courseId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var deletionResponse = await client.DeleteAsync($"{courseId}");
        deletionResponse.EnsureSuccessStatusCode();
    }


    private async Task<(string courseId, string moduleId)> CreateModule(InstructorUser instructor, string courseId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var moduleCreationResponse = await client.PostAsJsonAsync($"{courseId}/modules", GetValidCreateModuleCommand());

        moduleCreationResponse.EnsureSuccessStatusCode();

        var moduleId = (await moduleCreationResponse.Content.ReadFromJsonAsync<RequestResponse<string>>())?.Data;

        return (courseId, moduleId);
    }

    private async Task<(string courseId, string moduleId, string lectureId)> CreateLecture(InstructorUser instructor, string courseId, string moduleId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var lectureCreationResponse = await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", GetValidCreateLectureCommand("Article"));

        lectureCreationResponse.EnsureSuccessStatusCode();

        var lectureId = (await lectureCreationResponse.Content.ReadFromJsonAsync<RequestResponse<string>>())?.Data;

        return (courseId, moduleId, lectureId);
    }

    private async Task CreateLectureResourceContent(InstructorUser instructor, string resourceId)
    {

        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = ResourcesBaseUrl;

        var numberOfWord = new Random().Next(200, 1000);
        var randomText = new StringBuilder();

        for (int i = 0; i < numberOfWord; i++)
            randomText.Append(i).Append(' ');

        var response = await client.PutAsJsonAsync($"{resourceId}/article", new UpdateLectureArticleCommand { Content = randomText.ToString() });

        response.EnsureSuccessStatusCode();
    }
}
