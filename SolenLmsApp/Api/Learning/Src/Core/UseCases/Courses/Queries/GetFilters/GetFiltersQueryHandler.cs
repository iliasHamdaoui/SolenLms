using Imanys.SolenLms.Application.Learning.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.InstructorAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetFilters;

internal sealed class GetFiltersQueryHandler : IRequestHandler<GetFiltersQuery, RequestResponse<GetFiltersQueryResult>>
{
    private readonly IRepository<Instructor> _instructorRepository;
    private readonly IRepository<Category> _categoryRepository;

    public GetFiltersQueryHandler(IRepository<Instructor> instructorRepository, IRepository<Category> categoryRepository)
    {
        _instructorRepository = instructorRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<RequestResponse<GetFiltersQueryResult>> Handle(GetFiltersQuery query, CancellationToken cancellationToken)
    {
        var instructors = await _instructorRepository.ListAsync(new GetAllInstructorsSpec(), cancellationToken);
        var categories = await _categoryRepository.ListAsync(new GetAllCategoriesSpec(), cancellationToken);

        var instructorsToReturn = instructors.Select(x => new InstructorForGetFiltersQueryResult(x.Id, x.FullName));
        var categoriesToReturn = categories.Select(x => new CategoryForGetFiltersQueryResult(x.Id, x.Name));

        return RequestResponse<GetFiltersQueryResult>.Ok(data: new GetFiltersQueryResult { Instructors = instructorsToReturn, Categories = categoriesToReturn });
    }

    private class GetAllInstructorsSpec : Specification<Instructor>
    {
        public GetAllInstructorsSpec()
        {
            Query
            .AsNoTracking();
        }
    }

    private class GetAllCategoriesSpec : Specification<Category>
    {
        public GetAllCategoriesSpec()
        {
            Query
            .AsNoTracking();
        }
    }
}
