using Fluxor;
using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.CourseDetail.Store;

public sealed record CourseDetailState
{
    public GetCourseByIdQueryResult? CurrentCourse { get; init; }
    public string? CurrentModuleId { get; init; }
    public string? CurrentLectureId { get; init; }
    public ICollection<int>? CourseCategoriesIds { get; init; }
    public ICollection<CategoriesListItem>? Categories { get; init; }
    public ICollection<InstructorsListItem>? Instructors { get; init; }
    public ICollection<LearnerForGetCourseLearnersQuery>? CourseLearners { get; init; }
}

public sealed class CourseDetailFeatureState : Feature<CourseDetailState>
{
    public override string GetName() => nameof(CourseDetailState);

    protected override CourseDetailState GetInitialState()
    {
        return new CourseDetailState();
    }
}

