using Imanys.SolenLms.Application.Shared.Core.UseCases;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModulesOrders;

public sealed record UpdateModulesOrdersCommand : IRequest<RequestResponse>
{
    [JsonIgnore]
    public string CourseId { get; set; } = default!;
    public IEnumerable<ModuleOrder> ModulesOrders { get; set; } = default!;
}

