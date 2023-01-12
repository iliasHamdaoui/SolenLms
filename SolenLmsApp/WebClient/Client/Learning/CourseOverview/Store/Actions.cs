using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.CourseOverview.Store;


public sealed record LoadCourseAction(string CourseId, CancellationToken CancellationToken);
public sealed record LoadCourseResultAction(GetCourseByIdQueryResult Course);


