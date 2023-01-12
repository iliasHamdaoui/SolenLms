using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModule;

#nullable disable

public sealed record UpdateModuleCommand : IRequest<RequestResponse>
{
    public string ModuleTitle { get; set; }
    [JsonIgnore]
    public string CourseId { get; set; }
    [JsonIgnore]
    public string ModuleId { get; set; }
}
