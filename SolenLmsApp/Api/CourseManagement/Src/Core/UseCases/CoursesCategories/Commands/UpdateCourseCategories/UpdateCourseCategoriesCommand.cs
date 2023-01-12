using Imanys.SolenLms.Application.Shared.Core.UseCases;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Commands.UpdateCourseCategories;

#nullable disable

public sealed record UpdateCourseCategoriesCommand : IRequest<RequestResponse>
{
    public List<int> SelectecdCategroriesIds { get; set; }
    [JsonIgnore]
    public string CourseId { get; set; }
}
