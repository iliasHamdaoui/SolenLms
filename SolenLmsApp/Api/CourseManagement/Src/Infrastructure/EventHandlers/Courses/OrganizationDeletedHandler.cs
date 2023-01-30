using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Courses;

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
            int count = await DeleteAllOrganizationCoursesFromRepository(@event.OrganizationId, cancellationToken);

            _logger.LogInformation("Organization courses deleted. OrganizationId:{OrganizationId}, count:{count}",
                @event.OrganizationId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting organization courses. OrganizationId:{OrganizationId}, message:{message}",
                @event.OrganizationId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteAllOrganizationCoursesFromRepository(string organizationId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Courses.IgnoreQueryFilters().Where(x => x.OrganizationId == organizationId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}