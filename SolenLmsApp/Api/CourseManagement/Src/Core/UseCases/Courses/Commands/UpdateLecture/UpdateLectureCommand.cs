using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecture;

#nullable disable

public sealed record UpdateLectureCommand : IRequest<RequestResponse>
{
    public string LectureTitle { get; set; }
    [JsonIgnore]
    public string CourseId { get; set; }
    [JsonIgnore]
    public string ModuleId { get; set; }
    [JsonIgnore]
    public string LectureId { get; set; }
}
