using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, RequestResponse>
{
    private readonly IRepository<Category> _repo;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;
    private readonly IIntegratedEventsSender _eventsSender;

    public UpdateCategoryCommandHandler(IRepository<Category> repo, ILogger<UpdateCategoryCommandHandler> logger,
        IIntegratedEventsSender eventsSender)
    {
        _repo = repo;
        _logger = logger;
        _eventsSender = eventsSender;
    }

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