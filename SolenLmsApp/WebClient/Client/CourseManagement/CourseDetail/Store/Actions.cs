using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.CourseDetail.Store;

public sealed record LoadCourseAction(string CourseId, CancellationToken CancellationToken);
public sealed record LoadCourseResultAction(GetCourseByIdQueryResult Course);
public sealed record SetCurrentModuleAction(string? ModuleId);
public sealed record SetCurrentLectureAction(string? LectureId);
public sealed record LoadCourseCategoriesAction(string CourseId, CancellationToken CancellationToken);
public sealed record LoadCourseCategoriesResultAction(ICollection<CategoriesListItem> Categories, ICollection<int>? CourseCategoriesIds);
public sealed record LoadLearnersAction(string CourseId, CancellationToken CancellationToken);
public sealed record LoadLearnersResultAction(ICollection<LearnerForGetCourseLearnersQuery> Learners);



