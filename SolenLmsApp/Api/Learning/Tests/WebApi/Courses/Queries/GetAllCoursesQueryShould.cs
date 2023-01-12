using Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetAllCourses;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.AutoFixtureAttributs;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.Learning.Tests.WebApi.Courses.Queries;

[Collection("LearningWebApplicationFactory")]
public sealed class GetAllCoursesQueryShould
{
    private readonly LearningWebApplicationFactory _factory;

    public GetAllCoursesQueryShould(LearningWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.LearnerCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnEmptyCoursesCollectionWhenNoCoursesCreated(LearnerUser learner)
    {
        var client = await _factory.CreateClientWithUser(learner);

        var getAllCoursesResponse = await client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>($"");

        getAllCoursesResponse?.IsSuccess.Should().BeTrue();

        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().BeEmpty();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnTheExpectedCoursesCollectionWhenCoursesHaveBeenPublished(InstructorUser instructor,
        LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var expectedCourses = new List<CoursesListItem>
        {
            await CreateAndPublishCourse(instructor),
            await CreateAndPublishCourse(instructor),
            await CreateAndPublishCourse(instructor)
        };

        var client = await _factory.CreateClientWithUser(learner);

        var getAllCoursesResponse = await client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>($"");

        getAllCoursesResponse?.IsSuccess.Should().BeTrue();

        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().BeEquivalentTo(expectedCourses);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnOnlyCoursesBelongingToTheSameOrganizationAsTheLearner(InstructorUser instructor1,
        InstructorUser instructor2, LearnerUser learner)
    {
        learner.OrganizationId = instructor1.OrganizationId;
        await CreateAndPublishCourse(instructor1);
        await CreateAndPublishCourse(instructor1);
        await CreateAndPublishCourse(instructor1);

        await CreateAndPublishCourse(instructor2);

        var client = await _factory.CreateClientWithUser(learner);
        var getAllCoursesResponse = await client.GetFromJsonAsync<RequestResponse<GetAllCoursesQueryResult>>("");
        var courses = getAllCoursesResponse?.Data?.Courses;

        courses.Should().HaveCount(3,
            because: "we created 3 courses belonging to the same organization as the learner");
    }

    [Fact]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated()
    {
        var client = _factory.CreateAnonymousUserClient();

        var getAllCoursesResponse = await client.GetAsync("");
        getAllCoursesResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #region private methods

    private async Task<CoursesListItem> CreateAndPublishCourse(InstructorUser instructor)
    {
        var createdCourse = await _factory.CreateCourse(instructor);
        await _factory.PublishCourse(instructor, createdCourse.CourseId);

        return new CoursesListItem
        {
            Id = createdCourse.CourseId,
            Title = createdCourse.Title,
            Description = createdCourse.Description,
            Duration = createdCourse.Duration,
            InstructorName = createdCourse.InstructorName,
            PublicationDate = createdCourse.PublicationDate ?? default,
            Categories = Array.Empty<string>()
        };
    }

    #endregion
}