using Fluxor;
using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.Courses.Store;

public sealed record CoursesState
{
    public ICollection<CoursesListItem>? Courses { get; init; }
    public int CourseTotalCount { get; init; }
    public ICollection<CategoriesListItem>? Categories { get; init; }
    public ICollection<InstructorsListItem>? Instructors { get; init; }
    public GetAllCoursesQuery GetCoursesQuery { get; set; } = GetAllCoursesQuery.InitialState;
}

public sealed class CoursesFeatureState : Feature<CoursesState>
{
    public override string GetName() => nameof(CoursesState);

    protected override CoursesState GetInitialState()
    {
        return new CoursesState();
    }
}

public sealed record GetAllCoursesQuery(int Page, int PageSize, string OrderBy, bool IsSortDescending, string CategoriesIds, string InstructorsIds)
{
    public static GetAllCoursesQuery InitialState => new(1, 10, "lastUpdate", true, string.Empty, string.Empty);
}