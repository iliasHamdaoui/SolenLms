using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, RequestResponse>
{
    private readonly IRepository<Category> _repo;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    private readonly IIntegrationEventsSender _eventsSender;

    public CreateCategoryCommandHandler(IRepository<Category> repo, ICurrentUser currentUser,
        ILogger<CreateCategoryCommandHandler> logger, IIntegrationEventsSender eventsSender)
    {
        _repo = repo;
        _currentUser = currentUser;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    public async Task<RequestResponse> Handle(CreateCategoryCommand command, CancellationToken _)
    {
        try
        {
            Category newCategory = CreateNewCategory(command.CategoryName);

            await AddTheNewCategoryToRepository(newCategory);

            await SendCategoryCreatedEvent(newCategory);

            return Ok("The category has been created.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the category.", ex);
        }
    }

    #region private methods

    private Category CreateNewCategory(string categoryName)
    {
        return new Category(_currentUser.OrganizationId, categoryName);
    }
    private async Task AddTheNewCategoryToRepository(Category newCategory)
    {
        await _repo.AddAsync(newCategory);

        _logger.LogInformation("Category created. categoryId:{categoryId}", newCategory.Id);
    }

    private async Task SendCategoryCreatedEvent(Category createdCategory)
    {
        await _eventsSender.SendEvent(
            new CategoryCreated(createdCategory.OrganizationId, createdCategory.Id, createdCategory.Name));
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while creating a category. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}