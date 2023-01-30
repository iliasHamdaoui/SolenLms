using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Categories.Commands.CreateCategory;

#region Web Api

[Route("course-management/categories")]
[Authorize(Policy = CourseManagementPolicy)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Create a new course category
    /// </summary>
    /// <param name="command">Object containing information about the category to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> CreateCategory(CreateCategoryCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(60);
    }
}

#endregion

public sealed record CreateCategoryCommand : IRequest<RequestResponse>
{
    public string CategoryName { get; set; } = default!;
}

internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, RequestResponse>
{
    #region Constructor & Private properties

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

    #endregion

    public async Task<RequestResponse> Handle(CreateCategoryCommand createCategoryCommand, CancellationToken _)
    {
        try
        {
            Category newCategory = CreateNewCategory(createCategoryCommand.CategoryName);

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