using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Instructors;

internal sealed class OrganizationDeletedHandler : INotificationHandler<OrganizationDeleted>
{
    private readonly CourseManagementDbContext _dbContext;
    private readonly ILogger<OrganizationDeletedHandler> _logger;

    public OrganizationDeletedHandler(CourseManagementDbContext dbContext, ILogger<OrganizationDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(OrganizationDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteAllOrganizationInstructorsFromRepository(@event.OrganizationId, cancellationToken);

            _logger.LogInformation("Organization instructors deleted. OrganizationId:{OrganizationId}, count:{count}",
                @event.OrganizationId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting organization instructors. OrganizationId:{OrganizationId}, message:{message}",
                @event.OrganizationId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteAllOrganizationInstructorsFromRepository(string organizationId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Instructors
            .Where(x => x.OrganizationId == organizationId)
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}