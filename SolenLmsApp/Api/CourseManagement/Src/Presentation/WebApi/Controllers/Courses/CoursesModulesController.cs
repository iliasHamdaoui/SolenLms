using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateModule;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteModule;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModule;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModulesOrders;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.Courses;

#nullable disable

[Route("course-management/courses/{courseId}/modules")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class CoursesModulesController : BaseController
{
    /// <summary>
    /// Create a new training course module
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="command">Object containing information about the module to create</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> CreateModule(string courseId, CreateModuleCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        var response = await Mediator.Send(command, cancellationToken);

        if (response.IsSuccess)
            return CreatedAtAction(nameof(GetModuleById), new { courseId, moduleId = response.Data }, response);

        return new ActionResult<RequestResponse>(response);

    }

    /// <summary>
    /// Get a course module by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the module to get belongs</param>
    /// <param name="moduleId">The id of the module we want to get</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{moduleId}")]
    [ProducesResponseType(typeof(RequestResponse<GetModuleByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetModuleByIdQueryResult>>> GetModuleById(string courseId, string moduleId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetModuleByIdQuery(courseId, moduleId), cancellationToken));
    }

    /// <summary>
    /// Delete a course module by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the module to delete belongs</param>
    /// <param name="moduleId">The id of the module to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{moduleId}")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> DeleteModule(string courseId, string moduleId, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new DeleteModuleCommand(courseId, moduleId), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// update information about a course module
    /// </summary>
    /// <param name="courseId">The id of the course to which the module to update belongs</param>
    /// <param name="moduleId">The id of the module to update</param>
    /// <param name="command">Object containing information about the module to update.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpPut("{moduleId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateModule(string courseId, string moduleId, UpdateModuleCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;
        command.ModuleId = moduleId;

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// Update the order of a training course modules
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="command">Object containing information about the new order od the modules</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("order")]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateModuleOrders(string courseId, UpdateModulesOrdersCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}
