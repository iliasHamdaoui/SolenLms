using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery : IRequest<RequestResponse<GetAllCategoriesQueryResult>>
{
}
