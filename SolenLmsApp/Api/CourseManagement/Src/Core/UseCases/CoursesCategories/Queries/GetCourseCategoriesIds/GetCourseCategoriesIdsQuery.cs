using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Queries.GetCourseCategoriesIds;

public sealed record GetCourseCategoriesIdsQuery : IRequest<RequestResponse<List<int>>>
{
    public GetCourseCategoriesIdsQuery(string courseId)
    {
        CourseId = courseId;
    }

    public string CourseId { get; }
}
