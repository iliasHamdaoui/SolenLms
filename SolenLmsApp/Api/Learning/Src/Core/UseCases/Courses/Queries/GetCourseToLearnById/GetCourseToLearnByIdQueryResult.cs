#nullable disable

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseToLearnById;


public sealed record GetCourseToLearnByIdQueryResult
{
    public string CourseId { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }
    public string FirstLecture { get; set; }
    public float LearnerProgress { get; set; }
    public IEnumerable<ModuleForGetCourseToLearnByIdQueryResult> Modules { get; set; }
}

public sealed record ModuleForGetCourseToLearnByIdQueryResult
{
    public string Id { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public IEnumerable<LectureForGetCourseToLearnByIdQueryResult> Lectures { get; set; }
}
public sealed record LectureForGetCourseToLearnByIdQueryResult
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string LectureType { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public string ResourceId { get; set; }
    public string PreviousLectureId { get; set; }
    public string NextLectureId { get; set; }
    public string Content { get; set; }
}