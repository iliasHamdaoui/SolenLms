using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.CourseManagement.Features;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Features.Categories.Queries.GetAllCategories;

using static RequestResponse<GetAllCategoriesQueryResult>;
using Response = RequestResponse<GetAllCategoriesQueryResult>;

#region Web Api

[Route("course-management/categories")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get all the categories
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<GetAllCategoriesQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAllCategoriesQueryResult>> GetAllCategories(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetAllCategoriesQuery(), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetAllCategoriesQueryResult(List<CategoriesListItem> Categories);

public sealed record CategoriesListItem(int Id, string Name);

#endregion

public sealed record GetAllCategoriesQuery : IRequest<RequestResponse<GetAllCategoriesQueryResult>>
{
}

internal sealed class QueryHandler : IRequestHandler<GetAllCategoriesQuery, Response>
{
    #region Constructor & private members

    private readonly IRepository<Category> _repo;

    public QueryHandler(IRepository<Category> repo)
    {
        _repo = repo;
    }

    #endregion

    public async Task<Response> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
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

#region extensions

internal static class CategoryExtension
{
    public static CategoriesListItem ToCategoriesListItem(this Category category)
    {
        return new CategoriesListItem(category.Id, category.Name);
    }
}

#endregion