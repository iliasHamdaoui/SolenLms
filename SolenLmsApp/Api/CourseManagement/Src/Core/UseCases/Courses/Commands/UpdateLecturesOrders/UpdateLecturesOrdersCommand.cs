using Imanys.SolenLms.Application.Shared.Core.UseCases;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecturesOrders;


public sealed class UpdateLecturesOrdersCommand : IRequest<RequestResponse>
{
    [JsonIgnore]
    public string CourseId { get; set; } = default!;
    [JsonIgnore]
    public string ModuleId { get; set; } = default!;
    public IEnumerable<LectureOrder> LecturesOrders { get; set; } = default!;
}