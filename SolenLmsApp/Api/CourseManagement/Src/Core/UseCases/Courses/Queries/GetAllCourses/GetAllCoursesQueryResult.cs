namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQueryResult(List<CoursesListItem> Courses, int CourseTotalCount);

public sealed record CoursesListItem
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? InstructorName { get; set; }
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime LastUpdate { get; set; }
}
