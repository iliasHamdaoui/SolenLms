using Imanys.SolenLms.Application.Learning.Core.Domain.Categories;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Categories;

internal sealed class CategoryUpdatedHandler : INotificationHandler<CategoryUpdated>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<CategoryUpdatedHandler> _logger;


    public CategoryUpdatedHandler(LearningDbContext dbContext, ILogger<CategoryUpdatedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CategoryUpdated @event, CancellationToken cancellationToken)
    {
        Category? categoryToUpdate = await GetCategoryFromRepository(@event.CategoryId, cancellationToken);
        if (categoryToUpdate is null)
        {
            _logger.LogWarning("Category not found. categoryId:{categoryId}", @event.CategoryId);
            return;
        }

        try
        {
            categoryToUpdate.UpdateName(@event.CategoryName);

            await SaveChangesToRepository(cancellationToken);

            _logger.LogWarning("Category updated. categoryId:{categoryId}", @event.CategoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while updating category. categoryId:{categoryId}, message:{message}",
                @event.CategoryId, ex.Message);
        }
    }

    #region private methods

    private async Task<Category?> GetCategoryFromRepository(int categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == categoryId, cancellationToken);
    }

    private async Task SaveChangesToRepository(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}