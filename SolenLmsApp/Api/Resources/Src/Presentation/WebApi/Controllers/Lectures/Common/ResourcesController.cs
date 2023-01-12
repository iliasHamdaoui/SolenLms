using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.CheckLectureContent;
using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.GetLectureArticle;
using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.GetLectureVideo;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;

namespace Imanys.SolenLms.Application.Resources.WebApi.Controllers.Lectures.Common;


[Route("resources/lectures")]
[ApiExplorerSettings(GroupName = CourseManagementLearningGroupName)]
public sealed class ResourcesController : BaseController
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
        var response = await Mediator.Send(new GetLectureVideoQuery(resourceId), cancellationToken);

        return File(response.Data!, new MediaTypeHeaderValue("video/mp4").MediaType!, true);

    }

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

    /// <summary>
    /// Check whether the lecture content has been set
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{resourceId}/check")]
    [ProducesResponseType(typeof(RequestResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<bool>>> CheckLectureContent(string resourceId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new CheckLectureContentQuery(resourceId), cancellationToken));
    }
}
