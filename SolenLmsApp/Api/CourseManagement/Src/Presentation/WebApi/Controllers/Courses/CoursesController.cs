using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateCourse;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteCourse;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateCourse;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.PublishCourse;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UnpublishCourse;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.Courses;

#nullable disable

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class CoursesController : BaseController
{
    /// <summary>
    /// Get all the training courses
    /// </summary>
    /// <param name="query">the query to get courses</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<GetAllCoursesQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAllCoursesQueryResult>> GetAllCourses([FromQuery] GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// Get a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to get</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse<GetCourseByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetCourseByIdQueryResult>>> GetCourseById(string courseId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseByIdQuery(courseId), cancellationToken));
    }

    /// <summary>
    /// Create a new training course
    /// </summary>
    /// <param name="command">Object containing information about the training course to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse<string>>> CreateCourse(CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        var response = await Mediator.Send(command, cancellationToken);

        if (response.IsSuccess)
            return CreatedAtAction(nameof(GetCourseById), new { courseId = response.Data }, response);

        return new ActionResult<RequestResponse<string>>(response);
    }

    /// <summary>
    /// update information about a training course
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to update</param>
    /// <param name="command">Object containing information about the training course to update.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateCourse(string courseId, UpdateCourseCommand command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// Delete a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the course to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RequestResponse>> DeleteCourse(string courseId, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new DeleteCourseCommand(courseId), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// publish a training course
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to publish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content</returns>
    [HttpPut("{courseId}/publish")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> PublishCourse(string courseId, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new PublishCourseCommand(courseId), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// unpublish a training course
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to unpublish</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content</returns>
    [HttpPut("{courseId}/unpublish")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UnpublishCourse(string courseId,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new UnpublishCourseCommand(courseId), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }
}