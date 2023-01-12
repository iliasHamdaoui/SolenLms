using Imanys.SolenLms.Application.Learning.Core.UseCases.Bookmarks.Commands.ToggleBookmark;
using Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetAllCourses;
using Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseById;
using Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetFilters;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;

namespace Imanys.SolenLms.Application.Learning.WebApi.Controllers;

#nullable disable

[Route("courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
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
    public async Task<ActionResult<GetAllCoursesQueryResult>> GetAllCourses([FromQuery] GetAllCoursesQuery query, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// Get a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to get information about</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse<GetCourseByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetCourseByIdQueryResult>>> GetCourseById(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseByIdQuery(courseId), cancellationToken));
    }

    /// <summary>
    /// Get courses filters
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("filters")]
    [ProducesResponseType(typeof(RequestResponse<GetFiltersQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetFiltersQueryResult>>> GetFilters(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetFiltersQuery(), cancellationToken));
    }

    /// <summary>
    /// Toggle course bookmark
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to bookmark or unbookmark</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpPut("{courseId}/bookmark")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> ToggleBookmark(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new ToggleBookmarkCommand(courseId), cancellationToken));
    }

}
