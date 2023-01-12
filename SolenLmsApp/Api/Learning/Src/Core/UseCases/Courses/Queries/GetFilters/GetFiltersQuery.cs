using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetFilters;

public sealed record GetFiltersQuery : IRequest<RequestResponse<GetFiltersQueryResult>>
{
}
