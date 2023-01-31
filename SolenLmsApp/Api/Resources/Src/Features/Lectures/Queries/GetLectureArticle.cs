using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResources;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.Resources.Features.Lectures.Queries.GetLectureArticle;

#region Web API

[Route("resources/lectures")]
[ApiExplorerSettings(GroupName = CourseManagementLearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a lecture article
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("article/{resourceId}")]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse>> GetArticle(string resourceId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetLectureArticleQuery(resourceId), cancellationToken));
    }
}

#endregion

public sealed record GetLectureArticleQuery(string ResourceId) : IRequest<RequestResponse<string>>;

internal sealed class GetLectureArticleQueryHandler : IRequestHandler<GetLectureArticleQuery, RequestResponse<string>>
{
    #region Constructor

    private readonly IRepository<LectureResource> _repository;
    private readonly IHashids _hashids;

    public GetLectureArticleQueryHandler(IRepository<LectureResource> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    #endregion

    public async Task<RequestResponse<string>> Handle(GetLectureArticleQuery query, CancellationToken cancellationToken)
    {
        if (!_hashids.TryDecodeSingle(query.ResourceId, out int resourceId))
            return NotFound("Invalid resource id.");

        LectureResource? resource = await _repository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
            return NotFound("The resource does not exist.");

        if (resource.MediaType.Value != MediaType.Text.Value)
            return NotFound("The article format is incorrect.");

        return Ok(data: resource.Data);
    }
}