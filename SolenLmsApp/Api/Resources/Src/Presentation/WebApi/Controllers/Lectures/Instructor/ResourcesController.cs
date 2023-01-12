using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UpdateLectureArticle;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.WebApi.Controllers.Lectures.Instructor;

#nullable disable

[Route("resources/lectures/{resourceId}")]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class ResourcesController : BaseController
{
    /// <summary>
    /// Upload a lecture video resource
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="file">The video file to be uploaded</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [Authorize(Policy = CourseManagementPolicy)]
    [Consumes("multipart/form-data")]
    [HttpPut("video"), DisableRequestSizeLimit]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UploadVideo(string resourceId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null)
            return BadRequest();

        var response = await Mediator.Send(new UploadLectureVideoCommand(resourceId, new ResourceFile(file)), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// Update a lecture article content
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="command">Object containing the article content to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [Authorize(Policy = CourseManagementPolicy)]
    [HttpPut("article")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateContent(string resourceId, UpdateLectureArticleCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.ResourceId = resourceId;

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }
}
