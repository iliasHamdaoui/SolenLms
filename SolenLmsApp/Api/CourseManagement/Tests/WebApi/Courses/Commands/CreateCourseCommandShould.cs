using Imanys.SolenLms.Application.CourseManagement.Features.Courses.Commands.CreateCourse;
using Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.CourseManagement.Tests.WebApi.Courses.Commands;

[Collection("CourseManagementWebApplicationFactory")]
public class CreateCourseCommandShould
{
    private readonly CourseManagementWebApplicationFactory _factory;

    public CreateCourseCommandShould(CourseManagementWebApplicationFactory factory)
    {
        factory.ClientOptions.BaseAddress = factory.InstructorCoursesBaseUrl;
        _factory = factory;
    }

    [Theory]
    [InjectTestUser]
    public async Task CreateTheExpectedCourseWhenTheCommandIsValid(InstructorUser instructor)
    {
        var now = DateTime.Now;
        _factory.DateTimeMoq.Setup(x => x.Now).Returns(now);
        var client = await _factory.CreateClientWithUser(instructor);
        var command = _factory.GetValidCourseCreationCommand();

        var createCourseCommandResponse = await client.PostAsJsonAsync("", command);

        createCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getCourseByIdResult =
            await client.GetFromJsonAsync<RequestResponse<GetCourseByIdQueryResult>>(
                $"{createCourseCommandResponse.Headers.Location}");

        getCourseByIdResult.IsSuccess.Should().BeTrue();

        var createdCourse = getCourseByIdResult.Data;

        createdCourse.Title.Should().Be(command.CourseTitle);
        createdCourse.Description.Should().Be(command.CourseDescription);
        createdCourse.IsPublished.Should().BeFalse();
        createdCourse.InstructorId.Should().Be(instructor.Id);
        createdCourse.InstructorName.Should().Be($"{instructor.FamilyName} {instructor.GivenName}");
        createdCourse.CreatedAt.Should().Be(now);

        createdCourse.LastModifiedAt.Should().Be(now);
    }


    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenCommandIsNull(InstructorUser instructor)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        var createCourseCommandResponse = await client.PostAsJsonAsync<CreateCourseCommand>("", null);

        createCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnBadRequestWhenTheTitleIsInvalid(InstructorUser instructor, CreateCourseCommand command)
    {
        var client = await _factory.CreateClientWithUser(instructor);

        command.CourseTitle = null;

        var createCourseCommandResponse = await client.PostAsJsonAsync("", command);

        createCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InjectTestUser]
    public async Task CreateTheCourseWhenTheUserIsAdmin(AdminUser admin)
    {
        var client = await _factory.CreateClientWithUser(admin);

        var createCourseCommandResponse = await client.PostAsJsonAsync("", _factory.GetValidCourseCreationCommand());

        createCourseCommandResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InjectTestUser]
    public async Task ReturnForbiddendWhenTheUserIsLearner(LearnerUser learner)
    {
        var client = await _factory.CreateClientWithUser(learner);

        var createCourseCommandResponse = await client.PostAsJsonAsync("", _factory.GetValidCourseCreationCommand());

        createCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReturnUnauthorizedWhenTheUserIsNotAuthenticated()
    {
        var client = _factory.CreateAnonymousUserClient();

        var createCourseCommandResponse = await client.PostAsJsonAsync("", _factory.GetValidCourseCreationCommand());

        createCourseCommandResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}