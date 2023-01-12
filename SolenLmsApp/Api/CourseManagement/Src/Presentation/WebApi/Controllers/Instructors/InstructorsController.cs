using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Instructors.Queries.GetAllInstructors;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.Instructors;

[Route("course-management/instructors")]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class InstructorsController : BaseController
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
