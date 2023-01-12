using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Commands.UpdateCourseCategories;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Queries.GetCourseCategoriesIds;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.CoursesCategories;

[Route("course-management/courses/{courseId}/categories")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class CoursesCategoriesController : BaseController
{
    /// <summary>
    /// Get course categories ids
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<List<int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<List<int>>>> GetCourseCategoriesIds(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseCategoriesIdsQuery(courseId), cancellationToken));
    }

    /// <summary>
    /// update a course categories
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="command">Object containing the categories to add to the course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateCourseCategories(string courseId, UpdateCourseCategoriesCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }
}
