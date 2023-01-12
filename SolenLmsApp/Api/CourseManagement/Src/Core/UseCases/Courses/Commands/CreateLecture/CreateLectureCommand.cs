using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateLecture;

public sealed record CreateLectureCommand : IRequest<RequestResponse<string>>
{
    [JsonIgnore]
    public string CourseId { get; set; } = default!;
    [JsonIgnore]
    public string ModuleId { get; set; } = default!;
    public string LectureTitle { get; set; } = default!;
    public string LectureType { get; set; } = default!;
}
