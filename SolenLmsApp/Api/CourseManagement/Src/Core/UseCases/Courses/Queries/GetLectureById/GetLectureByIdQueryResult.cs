namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;

#nullable disable

public sealed record GetLectureByIdQueryResult
{
    public string Title { get; set; } 
    public string Type { get; set; }
    public string ResourceId { get; set; }
    public string MediaType { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
}
