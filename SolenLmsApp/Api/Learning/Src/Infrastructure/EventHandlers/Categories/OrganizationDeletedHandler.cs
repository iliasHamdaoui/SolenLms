﻿using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Categories;

internal sealed class OrganizationDeletedHandler : INotificationHandler<OrganizationDeleted>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<OrganizationDeletedHandler> _logger;

    public OrganizationDeletedHandler(LearningDbContext dbContext, ILogger<OrganizationDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(OrganizationDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteAllOrganizationCategoriesFromRepository(@event.OrganizationId, cancellationToken);

            _logger.LogInformation("Organization categories deleted. OrganizationId:{OrganizationId}, count:{count}",
                @event.OrganizationId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting organization categories. OrganizationId:{OrganizationId}, message:{message}",
                @event.OrganizationId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteAllOrganizationCategoriesFromRepository(string organizationId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .Where(x => x.OrganizationId == organizationId)
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}