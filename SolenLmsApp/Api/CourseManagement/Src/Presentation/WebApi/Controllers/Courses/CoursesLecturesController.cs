using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateLecture;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteLecture;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecture;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateLecturesOrders;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.Courses;


[Route("course-management/courses/{courseId}/modules/{moduleId}/lectures")]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class CoursesLecturesController : BaseController
{

    /// <summary>
    /// Create a new training course lecture
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="moduleId">The id of the module</param>
    /// <param name="command">Object containing information about the lecture to create</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> CreateLecture(string courseId, string moduleId, CreateLectureCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;
        command.ModuleId = moduleId;

        var response = await Mediator.Send(command, cancellationToken);

        if (response.IsSuccess)
            return CreatedAtAction(nameof(GetLectureById), new { courseId, moduleId, lectureId = response.Data }, response);

        return new ActionResult<RequestResponse>(response);

    }

    /// <summary>
    /// Get a course lecture by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the lecture to get belongs</param>
    /// <param name="moduleId">The id of the module to which the module to get belongs</param>
    /// <param name="lectureId">The id of the lecture we want to get</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{lectureId}")]
    [ProducesResponseType(typeof(RequestResponse<GetLectureByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetLectureByIdQueryResult>>> GetLectureById(string courseId, string moduleId, string lectureId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetLectureByIdQuery(courseId, moduleId, lectureId), cancellationToken));
    }

    /// <summary>
    /// Delete a course lecture by its id
    /// </summary>
    /// <param name="courseId">The id of the course to which the lecture to delete belongs</param>
    /// <param name="moduleId">The id of the module to which the lecture to delete belongs</param>
    /// <param name="lectureId">The id of the lecture to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{lectureId}")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> DeleteLecture(string courseId, string moduleId, string lectureId, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new DeleteLectureCommand(courseId, moduleId, lectureId), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// update information about a course lecture
    /// </summary>
    /// <param name="courseId">The id of the course to which the lecture to update belongs</param>
    /// <param name="moduleId">The id of the module to which the lecture to update belongs</param>
    /// <param name="lectureId">The id of the lecture to update</param>
    /// <param name="command">Object containing information about the lecture to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpPut("{lectureId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateLecture(string courseId, string moduleId, string lectureId, UpdateLectureCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;
        command.ModuleId = moduleId;
        command.LectureId = lectureId;

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// Update the order of a training course lectures
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="moduleId">The id of the module to which the lectures belong</param>
    /// <param name="command">Object containing information about the new order od the modules</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("order")]
    [Authorize(Policy = CourseManagementPolicy)]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateLectureOrders(string courseId, string moduleId, UpdateLecturesOrdersCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;
        command.ModuleId = moduleId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}
