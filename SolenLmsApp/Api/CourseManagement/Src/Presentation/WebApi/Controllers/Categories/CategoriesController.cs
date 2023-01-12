using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.CreateCategory;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.DeleteCategory;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.UpdateCategory;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Queries.GetAllCategories;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Imanys.SolenLms.Application.Shared.WebApi.OpenApiConstants;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.Categories;

[Route("course-management/categories")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class CategoriesController : BaseController
{
    /// <summary>
    /// Get all the categories
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<GetAllCategoriesQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAllCategoriesQueryResult>> GetAllCategories(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetAllCategoriesQuery(), cancellationToken));
    }

    /// <summary>
    /// Create a new course category
    /// </summary>
    /// <param name="command">Object containing information about the category to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> CreateCategory(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// update information about a course category
    /// </summary>
    /// <param name="categoryId">The id of the category we want to update</param>
    /// <param name="command">Object containing information about the category to update.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateCategory(int categoryId, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CategoryId = categoryId;

        var response = await Mediator.Send(command, cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }

    /// <summary>
    /// Delete a course category by its id
    /// </summary>
    /// <param name="categoryId">The id of the category to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{categoryId:int}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RequestResponse>> DeleteCategory(int categoryId, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken);

        return new ActionResult<RequestResponse>(response);
    }
}
