namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesQueryResult(List<CategoriesListItem> Categories);

public sealed record CategoriesListItem(int Id, string Name);
