using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Categories;

namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.CoursesCategories;
public sealed class CourseCategory : IAggregateRoot
{
    public int CourseId { get; init; }
    public Course Course { get; init; } = default!;
    public int CategoryId { get; init; }
    public Category Category { get; init; } = default!;

    public CourseCategory(int courseId, int categoryId)
    {
        CourseId = courseId;
        CategoryId = categoryId;
    }
}
