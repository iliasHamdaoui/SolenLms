using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;
using System.Collections.Generic;
using System.Linq;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Queries;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class GetCourseByIdQueryShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public GetCourseByIdQueryShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnExpectedTrainingCourseWhenValidCourseId(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var courseId = await _factory.CreateCourse(instructor);

        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");

        getCourseByIdResult.Should().NotBeNull();
        getCourseByIdResult?.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnTheExpectedCourseContentAndDuration(InstructorUser instructor)
    {
        HttpClient client = await _factory.CreateClientWithUser(instructor);
        var courseId = await _factory.CreateCourse(instructor);

        var expectedModules = await CreateModules(instructor, courseId);

        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");

        getCourseByIdResult.Data.Modules.Should().BeEquivalentTo(expectedModules);
        getCourseByIdResult.Data.Duration.Should().Be(expectedModules.Sum(x => x.Duration));
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheCourseIsInvalid(InstructorUser instructor, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var getCourseByIdResponse = await client.GetAsync($"{invalidCourseId}");

        getCourseByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheCourseHasBeenDeleted(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var courseId = await _factory.CreateCourse(instructor);

        await client.DeleteAsync(courseId);

        var getCourseByIdResponse = await client.GetAsync($"{courseId}");

        getCourseByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenCourseBelongsToAnOtherOrganization(InstructorUser instructor1,
        InstructorUser instructor2)
    {
        var courseId = await _factory.CreateCourse(instructor1);

        var client = await _factory.CreateClientWithUser(instructor2);

        var getCourseByIdResponse = await client.GetAsync($"{courseId}");

        getCourseByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnTheCourseWhenTheUserIsAdmin(InstructorUser instructor, AdminUser admin)
    {
        admin.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(admin);

        var getCourseByIdResponse = await client.GetAsync($"{courseId}");

        getCourseByIdResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseId = await _factory.CreateCourse(instructor);

        var client = await _factory.CreateClientWithUser(learner);

        var getCourseByIdResponse = await client.GetAsync($"{courseId}");

        getCourseByIdResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);

        var client = _factory.CreateAnonymousUserClient();

        var getCourseByIdResponse = await client.GetAsync($"{courseId}");

        getCourseByIdResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #region private methods

    public async Task<IEnumerable<ModuleForGetCourseByIdQueryResult>> CreateModules(InstructorUser instructor,
        string courseId)
    {
        var modules = new List<ModuleForGetCourseByIdQueryResult>();
        var random = new Random();
        var numberOfModules = random.Next(1, 3);

        for (int i = 0; i < numberOfModules; i++)
        {
            var client = await _factory.CreateClientWithUser(instructor);
            var (_, moduleId) = await _factory.CreateModule(instructor, courseId);
            var lectures = new List<LectureForGetCourseByIdQueryResult>();
            var numberOfLectures = random.Next(1, 3);
            for (int j = 0; j < numberOfLectures; j++)
            {
                var (_, _, lectureId) = await _factory.CreateLecture(instructor, null, courseId, moduleId);

                var getLectureByIdResult =
                    await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                        $"{courseId}/modules/{moduleId}/lectures/{lectureId}");

                await _factory.CreateLectureResourceContent(instructor, getLectureByIdResult.Data.ResourceId);

                getLectureByIdResult =
                    await client.GetFromJsonAsync<RequestResponse<GetLectureByIdQueryResult>>(
                        $"{courseId}/modules/{moduleId}/lectures/{lectureId}");
                var lecture = getLectureByIdResult.Data;

                lectures.Add(new LectureForGetCourseByIdQueryResult(lectureId, lecture.Title, lecture.Type,
                    lecture.Duration, lecture.Order, lecture.ResourceId));
            }

            var getModuleByIdResult =
                await client.GetFromJsonAsync<RequestResponse<GetModuleByIdQueryResult>>(
                    $"{courseId}/modules/{moduleId}");
            var module = getModuleByIdResult.Data;

            modules.Add(new ModuleForGetCourseByIdQueryResult
            {
                Id = moduleId,
                Title = module.Title,
                Duration = module.Duration,
                Order = module.Order,
                Lectures = lectures
            });
        }

        return modules;
    }

    #endregion
}