using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Categories;

internal sealed class CategoryDeletedHandler : INotificationHandler<CategoryDeleted>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<CategoryDeletedHandler> _logger;

    public CategoryDeletedHandler(LearningDbContext dbContext, ILogger<CategoryDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CategoryDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteCategoryFromRepository(@event.CategoryId, cancellationToken);

            _logger.LogInformation("Category deleted. categoryId:{categoryId}, count:{count}", @event.CategoryId,
                count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while deleting category. categoryId:{categoryId}, message:{message}",
                @event.CategoryId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteCategoryFromRepository(int categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .IgnoreQueryFilters()
            .Where(x => x.Id == categoryId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}