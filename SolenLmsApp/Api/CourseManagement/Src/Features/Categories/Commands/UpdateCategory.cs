using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Categories.Commands.UpdateCategory;

#region Web Api

[Route("course-management/categories")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
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
    public async Task<ActionResult<RequestResponse>> UpdateCategory(int categoryId, UpdateCategoryCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CategoryId = categoryId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(60);
    }
}

#endregion

public sealed record UpdateCategoryCommand : IRequest<RequestResponse>
{
    public string CategoryName { get; set; } = default!;
    [JsonIgnore] public int CategoryId { get; set; }
}

internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, RequestResponse>
{
    #region Constructor & Private properties

    private readonly IRepository<Category> _repo;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public UpdateCategoryCommandHandler(IRepository<Category> repo, ILogger<UpdateCategoryCommandHandler> logger,
        IIntegrationEventsSender eventsSender)
    {
        _repo = repo;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    #endregion
    
    public async Task<RequestResponse> Handle(UpdateCategoryCommand command, CancellationToken _)
    {
        try
        {
            Category? categoryToUpdate = await GetCategoryFromRepository(command.CategoryId);
            if (categoryToUpdate is null)
                return Error("The category does not exist.");

            categoryToUpdate.UpdateName(command.CategoryName);

            await SaveCategoryToRepository(categoryToUpdate);

            await SendCategoryUpdatedEvent(categoryToUpdate);

            return Ok("The category has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the category.", ex);
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

    private async Task SaveCategoryToRepository(Category categoryToUpdate)
    {
        await _repo.UpdateAsync(categoryToUpdate);

        _logger.LogInformation("Category updated. categoryId:{categoryId}", categoryToUpdate.Id);
    }

    private async Task SendCategoryUpdatedEvent(Category categoryToUpdate)
    {
        await _eventsSender.SendEvent(new CategoryUpdated(categoryToUpdate.Id, categoryToUpdate.Name));
    }

    private RequestResponse UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while updating the category. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}