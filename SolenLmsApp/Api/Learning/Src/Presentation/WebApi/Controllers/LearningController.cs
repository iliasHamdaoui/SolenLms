using Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetCourseToLearnById;
using Imanys.SolenLms.Application.Learning.Core.UseCases.LearnersProgress.Commands.UpdateLearnerProgress;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;

namespace Imanys.SolenLms.Application.Learning.WebApi.Controllers;

[Route("learning")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
public sealed class LearningController : BaseController
{
    /// <summary>
    /// Get a training course by its id
    /// </summary>
    /// <param name="courseId">The id of the training course that we want to learn</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(RequestResponse<GetCourseToLearnByIdQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetCourseToLearnByIdQueryResult>>> GetCourse(string courseId, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseToLearnByIdQuery(courseId), cancellationToken));
    }

    /// <summary>
    /// Update learner progress
    /// </summary>
    /// <param name="courseId">The id of the course</param>
    /// <param name="lectureId">The id of the last lecture the learner had access to</param>
    /// <param name="lastLecture">Indicate if this the last lecture of the course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpPut("{courseId}/{lectureId}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse>> UpdateLearnerProgress(string courseId, string lectureId, bool lastLecture, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new UpdateLearnerProgressCommand(courseId, lectureId, lastLecture), cancellationToken));
    }
}
