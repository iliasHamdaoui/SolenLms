using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Categories.Commands.DeleteCategory;

#region Web Api

[Route("course-management/categories")]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Delete a course category by its id
    /// </summary>
    /// <param name="categoryId">The id of the category to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{categoryId:int}")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RequestResponse>> DeleteCategory(int categoryId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken));
    }
}

#endregion

public sealed record DeleteCategoryCommand(int CategoryId) : IRequest<RequestResponse>;

internal sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, RequestResponse>
{
    #region Constructor & Private properties

    private readonly IRepository<Category> _repo;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public DeleteCategoryCommandHandler(IRepository<Category> repo, ILogger<DeleteCategoryCommandHandler> logger,
        IIntegrationEventsSender eventsSender)
    {
        _repo = repo;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    #endregion

    public async Task<RequestResponse> Handle(DeleteCategoryCommand command, CancellationToken _)
    {
        try
        {
            Category? categoryToDelete = await GetCategoryFromRepository(command.CategoryId);
            if (categoryToDelete is null)
                return Error("The category does not exist.");

            await DeleteCategoryFromRepository(categoryToDelete);

            await SendCategoryDeletedEvent(categoryToDelete);

            return Ok("The category has been deleted.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while deleting the category.", ex);
        }
    }


    #region private methods

    private async Task<Category?> GetCategoryFromRepository(int categoryId)
    {
        Category? category = await _repo.GetByIdAsync(categoryId);
        if (category is null)
            _logger.LogWarning("The category does not exist.");

        return category;
    }

    private async Task DeleteCategoryFromRepository(Category categoryToDelete)
    {
        await _repo.DeleteAsync(categoryToDelete);

        _logger.LogInformation("Category deleted. categoryId:{categoryId}", categoryToDelete.Id);
    }

    private async Task SendCategoryDeletedEvent(Category deletedCategory)
    {
        await _eventsSender.SendEvent(new CategoryDeleted(deletedCategory.Id));
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while deleting the category. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}