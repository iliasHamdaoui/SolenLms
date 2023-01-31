using Imanys.SolenLms.Application.Learning.Features.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.AutoFixtureAttributs;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.Learning.Tests.WebApi.Courses.Queries;

[Collection("LearningWebApplicationFactory")]
public sealed class GetCourseByIdQueryShould
{
    private readonly LearningWebApplicationFactory _factory;

    public GetCourseByIdQueryShould(LearningWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.LearnerCoursesBaseUrl;
        _factory = factory;
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnExpectedTrainingCourseWhenTheCourseIdIsValidAndTheCourseIsPublished(
        InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var expectedGetCourseByIdQueryResult = await CreateCourse(instructor);
        await _factory.PublishCourse(instructor, expectedGetCourseByIdQueryResult.CourseId);

        var client = await _factory.CreateClientWithUser(learner);
        var getCourseByIdResultResponse =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>(
                $"{expectedGetCourseByIdQueryResult.CourseId}");

        getCourseByIdResultResponse?.IsSuccess.Should().BeTrue();
        var getCourseByIdResult = getCourseByIdResultResponse!.Data;

        getCourseByIdResult.CourseId.Should().Be(expectedGetCourseByIdQueryResult.CourseId);
        getCourseByIdResult.Title.Should().Be(expectedGetCourseByIdQueryResult.Title);
        getCourseByIdResult.Description.Should().Be(expectedGetCourseByIdQueryResult.Description);
        getCourseByIdResult.Duration.Should().Be(expectedGetCourseByIdQueryResult.Duration);
        getCourseByIdResult.PublicationDate.Should().Be(expectedGetCourseByIdQueryResult.PublicationDate);
        getCourseByIdResult.InstructorName.Should().Be(expectedGetCourseByIdQueryResult.InstructorName);
        getCourseByIdResult.Modules.Should().BeEquivalentTo(expectedGetCourseByIdQueryResult.Modules,
            options => options.WithStrictOrdering());
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheCourseIdIsInvalid(LearnerUser learner, string invalidCourseId)
    {
        var client = await _factory.CreateClientWithUser(learner);
        var getCourseByIdResultResponse = await client.GetAsync($"{invalidCourseId}");

        getCourseByIdResultResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheCourseIsUnpublished(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseInfo = await CreateCourse(instructor);

        await _factory.PublishCourse(instructor, courseInfo.CourseId);
        await _factory.UnpublishCourse(instructor, courseInfo.CourseId);

        var client = await _factory.CreateClientWithUser(learner);
        var getCourseByIdResultResponse = await client.GetAsync($"{courseInfo.CourseId}");

        getCourseByIdResultResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenTheCourseIsDeleted(InstructorUser instructor, LearnerUser learner)
    {
        learner.OrganizationId = instructor.OrganizationId;
        var courseInfo = await CreateCourse(instructor);

        await _factory.PublishCourse(instructor, courseInfo.CourseId);
        await _factory.DeleteCourse(instructor, courseInfo.CourseId);

        var client = await _factory.CreateClientWithUser(learner);
        var getCourseByIdResultResponse = await client.GetAsync($"{courseInfo.CourseId}");

        getCourseByIdResultResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnNotFoundWhenCourseBelongsToAnOtherOrganization(InstructorUser instructor,
        LearnerUser learner)
    {
        var courseInfo = await CreateCourse(instructor);

        await _factory.PublishCourse(instructor, courseInfo.CourseId);

        var client = await _factory.CreateClientWithUser(learner);
        var getCourseByIdResultResponse = await client.GetAsync($"{courseInfo.CourseId}");

        getCourseByIdResultResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated(InstructorUser instructor)
    {
        var courseInfo = await CreateCourse(instructor);

        await _factory.PublishCourse(instructor, courseInfo.CourseId);

        var client = _factory.CreateAnonymousUserClient();
        var getCourseByIdResultResponse = await client.GetAsync($"{courseInfo.CourseId}");

        getCourseByIdResultResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #region private methods

    private async Task<GetCourseByIdQueryResult> CreateCourse(InstructorUser instructor)
    {
        var createdCourse = await _factory.CreateCourse(instructor);

        var result = new GetCourseByIdQueryResult
        {
            CourseId = createdCourse.CourseId,
            Title = createdCourse.Title,
            Description = createdCourse.Description,
            Duration = createdCourse.Duration,
            InstructorName = createdCourse.InstructorName,
            PublicationDate = createdCourse.PublicationDate ?? default,
            Modules = createdCourse.Modules.OrderBy(x => x.Order).Select(x => new ModuleForGetCourseByIdQueryResult
            {
                Id = x.Id,
                Title = x.Title,
                Duration = x.Duration,
                Order = x.Order,
                Lectures = x.Lectures.OrderBy(l => l.Order).Select(l => new LectureForGetCourseByIdQueryResult
                {
                    Id = l.Id,
                    Title = l.Title,
                    Order = l.Order,
                    Duration = l.Duration,
                    LectureType = l.LectureType
                }).ToList()
            }).ToList(),
        };
        return result;
    }

    #endregion
}