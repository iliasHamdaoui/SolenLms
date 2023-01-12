using Imanys.SolenLms.Application.Shared.Core.UseCases;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.UpdateCategory;

#nullable disable

public sealed record UpdateCategoryCommand : IRequest<RequestResponse>
{
    public string CategoryName { get; set; }
    [JsonIgnore]
    public int CategoryId { get; set; }
}
