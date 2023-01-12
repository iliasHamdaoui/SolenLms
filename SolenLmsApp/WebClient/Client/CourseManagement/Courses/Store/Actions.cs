using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.Courses.Store;

public sealed record LoadCoursesAction(CancellationToken CancellationToken);
public sealed record LoadCoursesResultAction(ICollection<CoursesListItem> Courses, int CourseTotalCount);
public sealed record LoadFiltersAction(CancellationToken CancellationToken);
public sealed record LoadFiltersResultAction(ICollection<CategoriesListItem> Categories, ICollection<InstructorsListItem> Instructors);
public sealed record SetCoursesListPageAction(int Page);
public sealed record SetCoursesListCategoriesAction(string CategoriesIds);
public sealed record SetCoursesListInstructorsAction(string InstructorsIds);
public sealed record SetCoursesListSortedColumnAction(string OrderBy, bool IsSortDescending);
public sealed record ResetCoursesListFiltersAction();