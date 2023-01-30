using Imanys.SolenLms.Application.Learning.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.InstructorAggregate;

namespace Imanys.SolenLms.Application.Learning.Features.Courses.Queries.GetFilters;

using static RequestResponse<GetFiltersQueryResult>;
using Response = RequestResponse<GetFiltersQueryResult>;


#region Web API

[Route("courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get courses filters
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("filters")]
    [ProducesResponseType(typeof(RequestResponse<GetFiltersQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetFiltersQueryResult>>> GetFilters(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetFiltersQuery(), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetFiltersQueryResult
{
    public IEnumerable<InstructorForGetFiltersQueryResult> Instructors { get; set; } = default!;
    public IEnumerable<CategoryForGetFiltersQueryResult> Categories { get; set; } = default!;
}

public sealed record InstructorForGetFiltersQueryResult
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public InstructorForGetFiltersQueryResult(string id, string name)
    {
        Id = id;
        Name = name;
    }
}

public sealed record CategoryForGetFiltersQueryResult
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public CategoryForGetFiltersQueryResult(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

#endregion

public sealed record GetFiltersQuery : IRequest<Response>;

internal sealed class GetFiltersQueryHandler : IRequestHandler<GetFiltersQuery, Response>
{
    #region Constructor

    private readonly IRepository<Instructor> _instructorRepository;
    private readonly IRepository<Category> _categoryRepository;

    public GetFiltersQueryHandler(IRepository<Instructor> instructorRepository,
        IRepository<Category> categoryRepository)
    {
        _instructorRepository = instructorRepository;
        _categoryRepository = categoryRepository;
    }

    #endregion
    
    public async Task<Response> Handle(GetFiltersQuery query, CancellationToken cancellationToken)
    {
        List<Instructor> instructors = await GetInstructorsFromRepository(cancellationToken);
        List<Category> categories = await GetCategoriesFromRepository(cancellationToken);

        IEnumerable<InstructorForGetFiltersQueryResult> instructorsToReturn =
            instructors.Select(x => new InstructorForGetFiltersQueryResult(x.Id, x.FullName));

        IEnumerable<CategoryForGetFiltersQueryResult> categoriesToReturn =
            categories.Select(x => new CategoryForGetFiltersQueryResult(x.Id, x.Name));

        return Ok(data: new GetFiltersQueryResult
        {
            Instructors = instructorsToReturn, Categories = categoriesToReturn
        });
    }
    
    #region private methods

    private async Task<List<Category>> GetCategoriesFromRepository(CancellationToken cancellationToken)
    {
        return await _categoryRepository.ListAsync(new GetAllCategoriesSpec(), cancellationToken);
    }

    private async Task<List<Instructor>> GetInstructorsFromRepository(CancellationToken cancellationToken)
    {
        return await _instructorRepository.ListAsync(new GetAllInstructorsSpec(), cancellationToken);
    }

    private sealed class GetAllInstructorsSpec : Specification<Instructor>
    {
        public GetAllInstructorsSpec()
        {
            Query
                .AsNoTracking();
        }
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