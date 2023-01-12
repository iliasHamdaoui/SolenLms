using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.LearnersProgress;

[Route("course-management/courses/{courseId}/learners")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class LearnersProgressController : BaseController
{
    /// <summary>
    /// Get course learners progress
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = CourseManagementPolicy)]
    [ProducesResponseType(typeof(RequestResponse<GetLearnersProgressQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetLearnersProgressQueryResult>>> GetLearnersProgress(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetLearnersProgressQuery(courseId), cancellationToken));
    }
}
