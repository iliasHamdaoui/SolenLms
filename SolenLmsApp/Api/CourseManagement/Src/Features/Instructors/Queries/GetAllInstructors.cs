using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Instructors;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Instructors.Queries.GetAllInstructors;

using static RequestResponse<GetAllInstructorsQueryResult>;
using Response = RequestResponse<GetAllInstructorsQueryResult>;

#region Web APi

[Route("course-management/instructors")]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get all the categories
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = CourseManagementPolicy)]
    [ProducesResponseType(typeof(RequestResponse<GetAllInstructorsQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAllInstructorsQueryResult>> GetAllInstructors(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetAllInstructorsQuery(), cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetAllInstructorsQueryResult(List<InstructorsListItem> Referents);

public sealed record InstructorsListItem(string Id, string Name);

#endregion

public sealed record GetAllInstructorsQuery : IRequest<RequestResponse<GetAllInstructorsQueryResult>>;

internal sealed class GetAllInstructorsQueryHandler : IRequestHandler<GetAllInstructorsQuery, Response>
{
    #region Constructor

    private readonly IRepository<Instructor> _repo;

    public GetAllInstructorsQueryHandler(IRepository<Instructor> repo)
    {
        _repo = repo;
    }

    #endregion
    
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

#region extensions

internal static class InstructorExtension
{
    public static InstructorsListItem ToInstructorItem(this Instructor instructor)
    {
        return new InstructorsListItem(instructor.Id, instructor.FullName);
    }
}

#endregion
