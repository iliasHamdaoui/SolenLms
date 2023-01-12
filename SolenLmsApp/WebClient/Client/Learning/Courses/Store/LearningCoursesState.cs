using Fluxor;
using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.Courses.Store;

public sealed record LearningCoursesState
{
    public ICollection<CoursesListItem>? Courses { get; init; }
    public int CourseTotalCount { get; init; }
    public ICollection<CategoryForGetFiltersQueryResult>? Categories { get; init; }
    public ICollection<InstructorForGetFiltersQueryResult>? Instructors { get; init; }

    public GetAllCoursesQuery GetCoursesQuery { get; set; } = GetAllCoursesQuery.InitialState;
}


public sealed class LearningCoursesFeatureState : Feature<LearningCoursesState>
{
    public override string GetName() => nameof(LearningCoursesState);

    protected override LearningCoursesState GetInitialState()
    {
        return new LearningCoursesState();
    }
}

public sealed record GetAllCoursesQuery(int Page, int PageSize, string OrderBy, string CategoriesIds, string InstructorsIds, bool BookmarkOnly)
{
    public static GetAllCoursesQuery InitialState => new(1, 10, "lastAccess", string.Empty, string.Empty, false);
}