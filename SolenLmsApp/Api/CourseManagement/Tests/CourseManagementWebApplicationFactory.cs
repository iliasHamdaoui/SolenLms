using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UpdateLectureArticle;
using Imanys.SolenLms.Application.Resources.Infrastructure.Storage.Local;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Imanys.SolenLms.Application.CourseManagement.Tests;

public sealed class CourseManagementWebApplicationFactory : CustomSharedWebApplicationFactory
{
    public override void ConfigureServices(IServiceCollection services)
    {

        services.Configure<LocalStorageSettings>(settings =>
        {
            settings.ResourcesFolder = TestResourcesFolder;
        });
    }


    public async Task<string> CreateCourse(InstructorUser instructor)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var response = await client.PostAsJsonAsync("", GetValidCourseCreationCommand());

        response.EnsureSuccessStatusCode();

        var courseId = (await response.Content.ReadFromJsonAsync<RequestResponse<string>>())?.Data;

        return courseId;
    }

    public async Task<(string courseId, string moduleId)> CreateModule(InstructorUser instructor, string courseId = null)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        courseId ??= await CreateCourse(instructor);

        var moduleCreationResponse = await client.PostAsJsonAsync($"{courseId}/modules", GetValidCreateModuleCommand());

        moduleCreationResponse.EnsureSuccessStatusCode();

        var moduleId = (await moduleCreationResponse.Content.ReadFromJsonAsync<RequestResponse<string>>())?.Data;

        return (courseId, moduleId);
    }

    public async Task<(string courseId, string moduleId, string lectureId)> CreateLecture(InstructorUser instructor, string lectureTypeValue = null, string courseId = null, string moduleId = null)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        courseId ??= await CreateCourse(instructor);

        if (moduleId == null)
            (_, moduleId) = await CreateModule(instructor, courseId);

        lectureTypeValue ??= "Article";

        var lectureCreationResponse = await client.PostAsJsonAsync($"{courseId}/modules/{moduleId}/lectures", GetValidCreateLectureCommand(lectureTypeValue));

        lectureCreationResponse.EnsureSuccessStatusCode();

        var lectureId = (await lectureCreationResponse.Content.ReadFromJsonAsync<RequestResponse<string>>())?.Data;

        return (courseId, moduleId, lectureId);
    }

    public async Task<GetLectureByIdQueryResult> GetLecture(TestUser instructor, string courseId, string moduleId, string lectureId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var getLectureByIdResult = await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        return getLectureByIdResult.Data;
    }

    public async Task DeleteLecture(InstructorUser instructor, string courseId, string moduleId, string lectureId)
    {
        var client = await this.CreateClientWithUser(instructor);
        client.BaseAddress = InstructorCoursesBaseUrl;

        var deleteLectureResult = await client.DeleteAsync($"{courseId}/modules/{moduleId}/lectures/{lectureId}");

        deleteLectureResult.EnsureSuccessStatusCode();
    }

    public async Task CreateLectureResourceContent(InstructorUser instructor, string resourceId)
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
