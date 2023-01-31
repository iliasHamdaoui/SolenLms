using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResources;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using System.Net.Http.Headers;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<System.IO.Stream>;

namespace Imanys.SolenLms.Application.Resources.Features.Lectures.Queries.GetLectureVideo;


#region Web Api

[Route("resources/lectures")]
[ApiExplorerSettings(GroupName = CourseManagementLearningGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get a lecture video
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("video/{resourceId}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVideo(string resourceId, CancellationToken cancellationToken)
    {
        RequestResponse<Stream> response = await Mediator.Send(new GetLectureVideoQuery(resourceId), cancellationToken);

        return File(response.Data!, new MediaTypeHeaderValue("video/mp4").MediaType!, true);
    }
}

#endregion
public sealed record GetLectureVideoQuery(string ResourceId) : IRequest<RequestResponse<Stream>>;

internal sealed class GetLectureVideoQueryHandler : IRequestHandler<GetLectureVideoQuery, RequestResponse<Stream>>
{
    #region Constructor

    private readonly IRepository<LectureResource> _repository;
    private readonly IMediaManager _videoManager;
    private readonly IHashids _hashids;

    public GetLectureVideoQueryHandler(IRepository<LectureResource> repository, IMediaManager videoManager,
        IHashids hashids)
    {
        _repository = repository;
        _videoManager = videoManager;
        _hashids = hashids;
    }

    #endregion
    
    public async Task<RequestResponse<Stream>> Handle(GetLectureVideoQuery query, CancellationToken cancellationToken)
    {
        if (!_hashids.TryDecodeSingle(query.ResourceId, out int resourceId))
            return NotFound("Invalid resource id.");

        LectureResource? resource = await _repository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
            return NotFound("The resource does not exist.");

        if (resource.MediaType.Value != MediaType.Video.Value)
            return NotFound("The video format is incorrect.");

        Stream? stream = await _videoManager.GetMediaContentStream(resource.Data);

        if (stream is null)
            return NotFound("The resource does not exist.");

        return Ok(data: stream);
    }
}