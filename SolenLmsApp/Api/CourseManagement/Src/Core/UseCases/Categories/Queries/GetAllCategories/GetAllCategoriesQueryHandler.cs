using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Queries.GetAllCategories.GetAllCategoriesQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Queries.GetAllCategories;

internal sealed class
    GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, RequestResponse<GetAllCategoriesQueryResult>>
{
    private readonly IRepository<Category> _repo;

    public GetAllCategoriesQueryHandler(IRepository<Category> repo)
    {
        _repo = repo;
    }

    public async Task<RequestResponse<GetAllCategoriesQueryResult>> Handle(GetAllCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        List<Category> categories = await GetCategoriesFromRepository(cancellationToken);

        List<CategoriesListItem> categoriesListItems = categories.Select(x => x.ToCategoriesListItem()).ToList();

        return Ok(data: new GetAllCategoriesQueryResult(categoriesListItems));
    }

    #region private methods
    
    private async Task<List<Category>> GetCategoriesFromRepository(CancellationToken cancellationToken)
    {
        return await _repo.ListAsync(new GetAllCategoriesSpec(), cancellationToken);
    }
    
    private sealed class GetAllCategoriesSpec : Specification<Category>
    {
        public GetAllCategoriesSpec()
        {
            Query
                .AsNoTracking();
        }
    }
    
    #endregion
}