using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, RequestResponse>
{
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