namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;

#nullable disable

public sealed record GetCourseByIdQueryResult
{
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string InstructorId { get; set; }
    public string InstructorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public IEnumerable<ModuleForGetCourseByIdQueryResult> Modules { get; set; }

}

public sealed record LectureForGetCourseByIdQueryResult(string Id, string Title, string LectureType, int Duration, int Order, string ResourceId);

public sealed record ModuleForGetCourseByIdQueryResult
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required int Duration { get; set; }
    public required int Order { get; set; }
    public required IEnumerable<LectureForGetCourseByIdQueryResult> Lectures{ get; set; }
}