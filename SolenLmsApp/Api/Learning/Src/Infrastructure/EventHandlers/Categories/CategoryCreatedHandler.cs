using Imanys.SolenLms.Application.Learning.Core.Domain.Categories;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Categories;

internal sealed class CategoryCreatedHandler : INotificationHandler<CategoryCreated>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<CategoryCreatedHandler> _logger;

    public CategoryCreatedHandler(LearningDbContext dbContext, ILogger<CategoryCreatedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CategoryCreated @event, CancellationToken cancellationToken)
    {
        try
        {
            Category newCategory = new(@event.OrganizationId, @event.CategoryId, @event.CategoryName);

            await AddNewCategoryToRepository(newCategory, cancellationToken);

            _logger.LogInformation("Category added. OrganizationId:{OrganizationId}", @event.OrganizationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while adding the category. OrganizationId:{OrganizationId}, message:{message}",
                @event.OrganizationId, ex.Message);
        }
    }

    #region private methods

    private async Task AddNewCategoryToRepository(Category categoryToAdd, CancellationToken cancellationToken)
    {
        _dbContext.Categories.Add(categoryToAdd);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}