using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateCourse;

#nullable disable

public sealed record UpdateCourseCommand : IRequest<RequestResponse>
{
    public string CourseTitle { get; set; }
    public string CourseDescription { get; set; }
    [JsonIgnore]
    public string CourseId { get; set; }
}
