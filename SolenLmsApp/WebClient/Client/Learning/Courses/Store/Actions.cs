using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.Courses.Store;

public sealed record LoadCoursesAction(CancellationToken CancellationToken);
public sealed record LoadCoursesResultAction(ICollection<CoursesListItem> Courses, int CourseTotalCount);
public sealed record LoadCoursesFiltersResultAction(ICollection<CategoryForGetFiltersQueryResult> Categories, ICollection<InstructorForGetFiltersQueryResult> Instructors);
public sealed record SetCoursesListPageAction(int Page);
public sealed record SetCoursesListCategoriesAction(string CategoriesIds);
public sealed record SetCoursesListInstructorsAction(string InstructorssIds);
public sealed record SetCoursesListSortedByAction(string OrderBy);
public sealed record SetCoursesListBookmarkOnlyAction(bool BookmarkOnly);
public sealed record ResetCoursesListFiltersAction();

