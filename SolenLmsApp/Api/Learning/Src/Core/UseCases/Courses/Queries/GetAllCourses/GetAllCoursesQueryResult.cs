namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQueryResult(List<CoursesListItem> Courses, int CourseTotalCount);

public sealed record CoursesListItem
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public DateTime PublicationDate { get; set; }
    public string? InstructorName { get; set; }
    public bool IsBookmarked { get; set; }
    public DateTime? LastAccess { get; set; }
    public IEnumerable<string> Categories { get; set; } = default!;
    public float LearnerProgress { get; set; }
}