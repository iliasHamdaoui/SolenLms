using Imanys.SolenLms.Application.CourseManagement.Core.Domain.InstructorAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
    CourseManagement.Core.UseCases.Instructors.Queries.GetAllInstructors.GetAllInstructorsQueryResult>;
using Response =
    Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.CourseManagement.Core.
        UseCases.Instructors.Queries.GetAllInstructors.GetAllInstructorsQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Instructors.Queries.GetAllInstructors;

internal sealed class
    GetAllInstructorsQueryHandler : IRequestHandler<GetAllInstructorsQuery, Response>
{
    private readonly IRepository<Instructor> _repo;

    public GetAllInstructorsQueryHandler(IRepository<Instructor> repo)
    {
        _repo = repo;
    }

    public async Task<Response> Handle(GetAllInstructorsQuery query, CancellationToken cancellationToken)
    {
        List<Instructor> instructors = await GetInstructorsFromRepository(cancellationToken);

        List<InstructorsListItem> instructorsListItems = instructors.Select(x => x.ToInstructorItem()).ToList();

        return Ok(data: new GetAllInstructorsQueryResult(instructorsListItems));
    }

    #region private methods

    private async Task<List<Instructor>> GetInstructorsFromRepository(CancellationToken cancellationToken)
    {
        return await _repo.ListAsync(new GetAllInstructorsSpec(), cancellationToken);
    }

    private sealed class GetAllInstructorsSpec : Specification<Instructor>
    {
        public GetAllInstructorsSpec()
        {
            Query
                .AsNoTracking();
        }
    }

    #endregion
}