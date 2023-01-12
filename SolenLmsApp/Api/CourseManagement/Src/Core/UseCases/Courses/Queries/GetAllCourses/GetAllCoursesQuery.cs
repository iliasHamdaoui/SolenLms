using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery : IRequest<RequestResponse<GetAllCoursesQueryResult>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = string.Empty;
    public bool IsSortDescending { get; set; } = false;
    public string CategoriesIds { get; set; } = string.Empty;
    public string InstructorsIds { get; set; } = string.Empty;
}
