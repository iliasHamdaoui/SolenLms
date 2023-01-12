namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;

#nullable disable

public sealed record GetModuleByIdQueryResult
{
    public string Title { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public IEnumerable<LectureForGetModuleByIdQueryResult> Lectures { get; set; }
}


public sealed record LectureForGetModuleByIdQueryResult
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string ResourceId { get; set; }
    public int Duration { get; set; }
}