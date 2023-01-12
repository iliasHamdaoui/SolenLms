using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateModule;

#nullable disable

public sealed record CreateModuleCommand : IRequest<RequestResponse<string>>
{
    public string ModuleTitle { get; set; }
    [JsonIgnore]
    public string CourseId { get; set; } = default!;
};
