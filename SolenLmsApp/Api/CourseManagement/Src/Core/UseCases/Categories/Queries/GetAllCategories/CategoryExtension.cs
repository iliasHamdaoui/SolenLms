using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Queries.GetAllCategories;
internal static class CategoryExtension
{
    public static CategoriesListItem ToCategoriesListItem(this Category category)
    {
        return new CategoriesListItem(category.Id, category.Name);
    }
}
