using Imanys.SolenLms.Application.Learning.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.CourseCategory;

public sealed class CourseCategory : IAggregateRoot
{
    public string CourseId { get; init; }
    public Course Course { get; init; } = default!;
    public int CategoryId { get; init; }
    public Category Category { get; init; } = default!;

    public CourseCategory(string courseId, int categoryId)
    {
        CourseId = courseId;
        CategoryId = categoryId;
    }
}