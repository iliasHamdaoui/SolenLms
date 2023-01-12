using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;
using System.Collections.Generic;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Queries;

[Collection("CourseManagementWebApplicationFactory")]
public sealed class GetAllCoursesQueryShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public GetAllCoursesQueryShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnEmptyCoursesCollectionWhenNoCoursesCreated(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var getAllCoursesResponse = await client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>($"");

        getAllCoursesResponse?.IsSuccess.Should().BeTrue();

        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().BeEmpty();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnTheExpectedCoursesCollectionWhenCoursesHaveBeenCreated(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var expectedCourses = new List<CoursesListItem>
        {
            await CreateCourse(instructor), await CreateCourse(instructor), await CreateCourse(instructor)
        };

        var getAllCoursesResponse = await client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>($"");

        getAllCoursesResponse?.IsSuccess.Should().BeTrue();

        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().BeEquivalentTo(expectedCourses);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNoneDeletedCourses(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var expectedCourses = new List<CoursesListItem>
        {
            await CreateCourse(instructor), await CreateCourse(instructor), await CreateCourse(instructor)
        };

        var courseToBeDeleted = await CreateCourse(instructor);

        await client.DeleteAsync($"{courseToBeDeleted.Id}");

        var getAllCoursesResponse = await client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>($"");

        getAllCoursesResponse?.IsSuccess.Should().BeTrue();

        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().BeEquivalentTo(expectedCourses);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnOkWhenTheUserIsAdmin(AdminUser admin)
    {
        var client = await _factory.CreateClientWithUser(admin);

        var getAllCoursesResponse = await client.GetAsync("");

        getAllCoursesResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddenWhenTheUserIsLearner(LearnerUser learner)
    {
        var client = await _factory.CreateClientWithUser(learner);

        var getAllCoursesResponse = await client.GetAsync("");
        getAllCoursesResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated()
    {
        var client = _factory.CreateAnonymousUserClient();

        var getAllCoursesResponse = await client.GetAsync("");
        getAllCoursesResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnOnlyCoursesBelongingToTheSameOrganizationAsTheInstructor(InstructorUser instructor1,
        InstructorUser instructor2)
    {
        await CreateCourse(instructor1);
        await CreateCourse(instructor1);
        await CreateCourse(instructor1);

        await CreateCourse(instructor2);

        var instructor1Client = await _factory.CreateClientWithUser(instructor1);
        var getAllCoursesResponse =
            await instructor1Client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>("");
        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().HaveCount(3,
            because: "we created 3 courses belonging to the same orgaization as the instructor");
    }

    #region private methods

    private async Task<CoursesListItem> CreateCourse(InstructorUser instructor)
    {
        var courseId = await _factory.CreateCourse(instructor);
        var client = await _factory.CreateClientWithUser(instructor);
        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>($"{courseId}");

        var course = getCourseByIdResult?.Data;

        return new CoursesListItem
        {
            Id = course!.CourseId,
            Title = course.Title,
            Description = course.Description,
            InstructorName = course.InstructorName,
            IsPublished = course.IsPublished,
            Duration = course.Duration,
            LastUpdate = course.LastModifiedAt
        };
    }

    #endregion
}