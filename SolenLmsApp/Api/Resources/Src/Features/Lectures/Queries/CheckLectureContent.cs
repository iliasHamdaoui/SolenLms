using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<bool>;

namespace Imanys.SolenLms.Application.Resources.Features.Lectures.Queries.CheckLectureContent;

#region Web API

[Route("resources/lectures")]
[ApiExplorerSettings(GroupName = CourseManagementLearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Check whether the lecture content has been set
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{resourceId}/check")]
    [ProducesResponseType(typeof(RequestResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<bool>>> CheckLectureContent(string resourceId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new CheckLectureContentQuery(resourceId), cancellationToken));
    }
}

#endregion

public sealed record CheckLectureContentQuery(string ResourceId) : IRequest<RequestResponse<bool>>;

internal sealed class CheckLectureContentQueryHandler : IRequestHandler<CheckLectureContentQuery, RequestResponse<bool>>
{
    #region Constructor

    private readonly IRepository<LectureResource> _repository;
    private readonly IHashids _hashids;

    public CheckLectureContentQueryHandler(IRepository<LectureResource> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    #endregion

    public async Task<RequestResponse<bool>> Handle(CheckLectureContentQuery query, CancellationToken cancellationToken)
    {
        if (!_hashids.TryDecodeSingle(query.ResourceId, out int resourceId))
            return NotFound("Invalid resource id.");

        LectureResource? resource = await _repository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
            return NotFound("The resource does not exist.");

        return Ok(data: resource.Data != null);
    }
}