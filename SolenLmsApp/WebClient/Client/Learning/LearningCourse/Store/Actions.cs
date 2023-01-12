using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.LearningCourse.Store;

public sealed record LoadLearningCourseAction(string CourseId, CancellationToken CancellationToken);
public sealed record LoadLearningCourseResultAction(GetCourseToLearnByIdQueryResult Course);
public sealed record LoadLectureAction(string CourseId, string LectureId, CancellationToken CancellationToken);
public sealed record LoadLectureResultAction(LectureForGetCourseToLearnByIdQueryResult? Lecture);