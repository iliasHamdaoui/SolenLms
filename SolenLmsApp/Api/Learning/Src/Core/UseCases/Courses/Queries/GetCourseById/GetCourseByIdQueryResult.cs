namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseById;

#nullable disable

public sealed record GetCourseByIdQueryResult
{
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public DateTime PublicationDate { get; set; }
    public string InstructorName { get; set; }
    public bool IsBookmarked { get; set; }
    public float LearnerProgress { get; set; }
    public IEnumerable<string> Categories { get; set; } = default!;
    public IEnumerable<ModuleForGetCourseByIdQueryResult> Modules { get; set; }
}

public sealed record ModuleForGetCourseByIdQueryResult {
    public string Id { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public IEnumerable<LectureForGetCourseByIdQueryResult> Lectures { get; set; }
}
public sealed record LectureForGetCourseByIdQueryResult
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string LectureType { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
}

